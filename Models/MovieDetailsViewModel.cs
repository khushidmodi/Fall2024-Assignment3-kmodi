using System;
using System.Collections.Generic;

namespace Fall2024_Assignment3_kmodi.Models
{
    //public class MovieDetailsViewModel
    //{
    //    public Movie Movie { get; set; }
    //    public IEnumerable<Actor> Actors { get; set; }

    //    public MovieDetailsViewModel(Movie movie, IEnumerable<Actor> actors)
    //    {
    //        Movie = movie;
    //        Actors = actors;
    //    }
    //}

    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; }
        public IEnumerable<Actor> Actors { get; set; }

        // New properties for AI-generated reviews and sentiment analysis
        public List<Object[]> ReviewsAndSentiments { get; set; }
        public double AvgSentiment { get; set; } // Overall sentiment score for the movie

        public MovieDetailsViewModel(Movie movie, IEnumerable<Actor> actors, List<Object[]> reviews_and_sentiments, double avg)
        {
            Movie = movie;
            Actors = actors;
            ReviewsAndSentiments = reviews_and_sentiments;
            AvgSentiment = avg;
        }
    }
}

