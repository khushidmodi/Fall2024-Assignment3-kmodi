﻿using System;
namespace Fall2024_Assignment3_kmodi.Models
{
	public class ActorDetailsViewModel
	{

        public Actor Actor { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
        public List<Object[]> TweetsAndSentiments { get; set; }
        public double AvgSentiment { get; set; }

        public ActorDetailsViewModel(Actor actor, IEnumerable<Movie> movies, List<Object[]> tweets_and_sentiments, double avg)
        {
            Actor = actor;
            Movies = movies;
            TweetsAndSentiments = tweets_and_sentiments;
            AvgSentiment = avg;
        }

    }
}

