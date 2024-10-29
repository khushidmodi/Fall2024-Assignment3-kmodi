using System;
namespace Fall2024_Assignment3_kmodi.Models
{
	public class Movie
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImdbLink { get; set; }
        public string Genre { get; set; }
        public int YearOfRelease { get; set; }
        //public string PosterUrl { get; set; }
        public byte[]? Photo { get; set; }

        // Navigation property for actors in the movie
        //public ICollection<MovieActor> MovieActors { get; set; }
    }
}

