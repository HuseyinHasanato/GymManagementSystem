using GymManagementSystem.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace GymManagementSystem.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // التأكد من جلب المفتاح من الإعدادات
            _apiKey = configuration["GeminiKey"] ?? "";
        }

        public async Task<string> GenerateWorkoutPlanAsync(UserProfile profile)
        {
            if (string.IsNullOrEmpty(_apiKey))
                return "❌ Gemini API Key eksik! appsettings.json dosyasını kontrol edin.";

            // استخدام v1beta للوصول إلى أحدث النماذج
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";

            var promptText = new StringBuilder();
            promptText.AppendLine("Sen profesyonel bir fitness antrenörü ve diyetisyensin.");
            promptText.AppendLine($"Müşteri Bilgileri: Yaş: {profile.Age}, Boy: {profile.HeightCm}cm, Kilo: {profile.WeightKg}kg.");
            promptText.AppendLine($"Hedef: {profile.FitnessGoal}.");
            promptText.AppendLine("Lütfen 5 günlük detaylı antrenman programı ve 3 beslenme tavsiyesi içeren Türkçe bir yanıt ver. Markdown formatını kullan.");

            var requestBody = new
            {
                contents = new[] {
                    new {
                        parts = new[] {
                            new { text = promptText.ToString() }
                        }
                    }
                }
            };

            try
            {
                // إرسال الطلب مع التحقق من الوقت
                var response = await _httpClient.PostAsJsonAsync(url, requestBody);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseString);

                    // محاولة استخراج النص بأمان
                    if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                    {
                        var text = candidates[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text")
                            .GetString();

                        return text ?? "⚠️ AI yanıtı metin içermiyor.";
                    }

                    return "⚠️ AI yanıt yapısı beklenenden farklı.";
                }

                // في حال وجود خطأ من سيرفر Google (مثل 400 أو 403 أو 429)
                return $"❌ Google API Hatası ({response.StatusCode}): {responseString}";
            }
            catch (HttpRequestException httpEx)
            {
                return $"❌ Bağlantı Hatası: İnternet bağlantınızı veya API adresini kontrol edin. Detay: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                return $"❌ Beklenmedik Sistem Hatası: {ex.Message}";
            }
        }
    }
}