using GymManagementSystem.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json; // مطلوب للتعامل مع JSON
using System.Net.Http.Headers; // مطلوب لإضافة مفتاح API

namespace GymManagementSystem.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiKey = _configuration["OpenAISettings:ApiKey"] ?? throw new ArgumentNullException("OpenAI API Key is missing in configuration.");

            // إعداد رأس (Header) المصادقة لجميع الطلبات
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        }

        public async Task<string> GenerateWorkoutPlanAsync(UserProfile profile)
        {
            // بناء النص الذي سيتم إرساله إلى نموذج الذكاء الاصطناعي (Prompt)
            string prompt = $"Yaşım {profile.Age}, kilom {profile.WeightKg} kg, boyum {profile.HeightCm} cm ve fitness hedefim {profile.FitnessGoal}. " +
                 "Hedefime odaklanan, detaylı bir 5 günlük egzersiz planı oluştur. Yanıt, TÜRKÇE olmalı ve okunaklı olması için Markdown kullanılarak açıkça biçimlendirilmelidir.";

            try
            {
                // بناء طلب JSON (باستخدام نموذج GPT-3.5-turbo الأسرع)
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

                // إرسال الطلب إلى نقطة نهاية Chat Completions
                var response = await _httpClient.PostAsync("chat/completions", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // تحليل الرد للحصول على محتوى الرسالة
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        var messageContent = doc.RootElement.GetProperty("choices")[0]
                                                .GetProperty("message").GetProperty("content").GetString();
                        return messageContent ?? "فشلت عملية تحليل الرد من الذكاء الاصطناعي.";
                    }
                }

                // في حالة فشل الاتصال
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"عذراً، فشلت عملية توليد الخطة التدريبية. رمز الخطأ: {response.StatusCode}. التفاصيل: {errorContent}";
            }
            catch (Exception ex)
            {
                return $"حدث خطأ في الاتصال بالذكاء الاصطناعي: {ex.Message}";
            }
        }
    }
}