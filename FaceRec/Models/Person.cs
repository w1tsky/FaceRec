namespace FaceRec.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserData { get; set; }
        public byte[] PersonPhoto { get; set; }

        
        public Person(string firstName, string lastName, byte[] personPhoto)
        {
            FirstName = firstName;
            LastName = lastName;
            PersonPhoto = personPhoto;
        }

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
           
        }

        public Person(int id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;

        }
    }

}
