using GymManagementSystem.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace GymManagementSystem.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly bool _isServiceEnabled; // متغير لتتبع حالة تمكين الخدمة

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // محاولة قراءة المفتاح
            _apiKey = configuration["GeminiSettings:ApiKey"];

            if (string.IsNullOrEmpty(_apiKey) || _apiKey.Contains("YOUR_"))
            {
                // إذا كان المفتاح مفقوداً أو وهمياً، قم بتعطيل الخدمة
                _isServiceEnabled = false;
                // يمكنك طباعة رسالة للمطور في نافذة Console
                Console.WriteLine("⚠️ تحذير: مفتاح AI غير موجود. خدمة الذكاء الاصطناعي معطلة.");
            }
            else
            {
                // إذا كان المفتاح موجوداً
                _isServiceEnabled = true;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            }
        }

        public async Task<string> GenerateWorkoutPlanAsync(UserProfile profile)
        {
            // التحقق أولاً من تمكين الخدمة
            if (!_isServiceEnabled)
            {
                return "⚠️ **عذراً، خدمة توليد خطة التدريب غير متاحة حالياً.** يرجى الاتصال بإدارة الصالة الرياضية لتفعيل الخدمة.";
            }

            // ********** المنطق الأصلي لتوليد الخطة **********
            string prompt = $"Yaşım {profile.Age}, kilom {profile.WeightKg} kg, boyum {profile.HeightCm} cm ve fitness hedefim {profile.FitnessGoal}. " +
                            "Hedefime odaklanan, detaylı bir 5 günlük egzersiz planı oluştur. Yanıt, TÜRKÇE olmalı ve okunaklı olması için Markdown kullanılarak açıkça biçimlendirilmelidir.";

            try
            {
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "أنت مدرب لياقة شخصي وخبير في التغذية. مهمتك هي توليد خطة تمارين بناءً على البيانات المقدمة."},
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 2500,
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("chat/completions", content);

                if (response.IsSuccessStatusCode)
                {
                    // تحليل الرد
                    using (JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
                    {
                        var messageContent = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                        return messageContent ?? "فشلت عملية تحليل الرد من الذكاء الاصطناعي.";
                    }
                }

                // رسالة الخطأ في حالة المفتاح غير الصالح أو نفاد الرصيد
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"عذراً، فشلت عملية توليد الخطة. رمز الخطأ: {response.StatusCode}. (قد يكون المفتاح غير صحيح أو لا يوجد رصيد). التفاصيل: {errorContent}";
            }
            catch (Exception ex)
            {
                return $"حدث خطأ في الاتصال بالذكاء الاصطناعي: {ex.Message}";
            }
        }
    }
}