//using System;
using System.ComponentModel.DataAnnotations;
namespace Fall2024_Assignment3_kmodi.Models
{
	public class Actor
	{
        public int Id { get; set; }

        //public required string Name { get; set; }

        //public string? Name { get; set; }


        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string ImdbLink { get; set; }
        public byte[]? Photo { get; set; }

        // Navigation property for movies the actor appeared in
        //public ICollection<MovieActor> MovieActors { get; set; }
    }
}

