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
            // قراءة المفتاح من appsettings.json
            _apiKey = configuration["GeminiKey"] ?? "";
        }

        public async Task<string> GenerateWorkoutPlanAsync(UserProfile profile)
        {
            if (string.IsNullOrEmpty(_apiKey))
                return "❌ Gemini API Key eksik! Lütfen appsettings.json dosyasını kontrol edin.";

            // استخدام موديل Gemini 1.5 Flash (الأسرع والأفضل للمشاريع الطلابية)
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";

            // بناء الطلب (Prompt) بطريقة احترافية لضمان جودة الجدول
            var promptText = new StringBuilder();
            promptText.AppendLine("Sen uzman bir fitness eğitmeni ve beslenme uzmanısın.");
            promptText.AppendLine($"Kullanıcı Profili: Yaş {profile.Age}, Boy {profile.HeightCm}cm, Kilo {profile.WeightKg}kg.");
            promptText.AppendLine($"Ana Hedef: {profile.FitnessGoal}.");
            promptText.AppendLine("Lütfen aşağıdaki kriterlere göre profesyonel bir program hazırla:");
            promptText.AppendLine("1. 5 günlük detaylı antrenman planı.");
            promptText.AppendLine("2. Her gün için set ve tekrar sayıları.");
            promptText.AppendLine("3. Hedefe uygun kısa beslenme tavsiyeleri.");
            promptText.AppendLine("4. Yanıtı tamamen TÜRKÇE ve şık bir Markdown formatında ver.");

            var requestBody = new
            {
                contents = new[] {
                    new {
                        parts = new[] {
                            new { text = promptText.ToString() }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7, // توازن بين الإبداع والدقة
                    maxOutputTokens = 2048
                }
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();

                    // استخراج النص مع التحقق من وجود البيانات لتجنب الـ Null Reference
                    if (result.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                    {
                        var content = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                        return content ?? "Hata: İçerik oluşturulamadı.";
                    }
                    return "❌ AI yanıt üretemedi, lütfen tekrar deneyin.";
                }

                // معالجة رسائل الخطأ الشائعة
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    return "❌ Hata: Geçersiz istek veya API anahtarı hatası.";

                return $"❌ AI Hatası: {response.StatusCode}";
            }
            catch (Exception ex)
            {
                return $"❌ Sistem Hatası: {ex.Message}";
            }
        }
    }
}