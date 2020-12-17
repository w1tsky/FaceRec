namespace FaceRec.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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
    }

}
