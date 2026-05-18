namespace Kurban.Models
{
    public class Breed
    {
        public int Id { get; set; } //Pk
        public string BreedName { get; set; } // koyun, keçi, sığır, deve
                                              //bir türün birden fazla cinsi olabilir. Örneğin, koyun türünün merinos, dağlıç, karakaş gibi cinsleri vardır.
                                              //Bu nedenle, Breed sınıfında bir türün birden fazla cinsini temsil etmek için bir koleksiyon ekleyebiliriz.
        public ICollection<Animal>? Animals { get; set; } // Soru işareti 'boş olabilir' demek.    }
    }
}
