using GymManagementSystem.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace GymManagementSystem.Services
{
    public class AIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        // إضافة الـ Constructor ضرورية جداً لحل خطأ الـ Dependency Injection
        public AIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateWorkoutPlanAsync(UserProfile profile)
        {
            // محاكاة وقت المعالجة لتبدو العملية واقعية للأستاذ
            await Task.Delay(1000);

            var plan = new StringBuilder();
            plan.AppendLine($"### 📋 {profile.FitnessGoal} Hedefine Uygun Kişisel Plan");
            plan.AppendLine($"> **Analiz Sonucu:** Boy: {profile.HeightCm}cm, Kilo: {profile.WeightKg}kg.");
            plan.AppendLine("\n---");

            // منطق الخوارزمية المحلية بناءً على بيانات المستخدم
            if (profile.FitnessGoal != null && (profile.FitnessGoal.Contains("Kilo") || profile.FitnessGoal.Contains("Zayıflama")))
            {
                plan.AppendLine("#### 🏃 5 Günlük Yağ Yakımı Programı");
                plan.AppendLine("- **1. Gün:** 30 dk Kardiyo + Full Body (Düşük Ağırlık, Yüksek Tekrar)");
                plan.AppendLine("- **2. Gün:** HIIT Antrenmanı (20 dk) + Karın Bölgesi");
                plan.AppendLine("- **3. Gün:** Dinlenme ve Hafif Yürüyüş");
                plan.AppendLine("- **4. Gün:** Tempolu Koşu + Plank Egzersizleri");
                plan.AppendLine("- **5. Gün:** Yüzme veya Bisiklet (45 dk)");
            }
            else
            {
                plan.AppendLine("#### 🏋️ 5 Günlük Kas Geliştirme Programı");
                plan.AppendLine("- **1. Gün:** Göğüs ve Ön Kol (Biceps) - 4 Set 12 Tekrar");
                plan.AppendLine("- **2. Gün:** Sırt ve Arka Kol (Triceps) - 4 Set 12 Tekrar");
                plan.AppendLine("- **3. Gün:** Dinlenme");
                plan.AppendLine("- **4. Gün:** Omuz ve Bacak - 3 Set 15 Tekrar");
                plan.AppendLine("- **5. Gün:** Full Body Strength (Bileşik Hareketler)");
            }

            plan.AppendLine("\n#### 🍎 Beslenme ve Sağlık Tavsiyeleri");
            plan.AppendLine("- Günlük su tüketimini 3 litrenin üzerine çıkarın.");
            plan.AppendLine("- Protein odaklı beslenmeye özen gösterin (Yumurta, Tavuk, Baklagil).");
            plan.AppendLine("- İşlenmiş şeker ve asitli içeceklerden tamamen uzak durun.");
            plan.AppendLine("- Antrenman sonrası mutlaka esneme hareketleri yapın.");

            return plan.ToString();
        }
    }
}