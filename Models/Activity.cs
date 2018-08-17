using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginRegistration.Models
{
    public class Activity{
        
        [Key]
        public int ActivityId {get;set;}
        [Required]
        public string name {get;set;}
        [Required]
        public DateTime date {get;set;}
        [Required]
        public int time {get;set;}
        public string timeType {get;set;}
        [Required]
        public int duration {get;set;}
        public string durationtype{get;set;}
        [Required]
        [MinLength(10)]
        public string description {get;set;}
        public DateTime created_at {get;set;}
        public DateTime updated_at {get;set;}
        [ForeignKey("useid")]
        public int PlannerId {get;set;}
        public User planner {get;set;}
        public List<Participant> guests {get;set;}
        public Activity()
        {
            guests = new List<Participant>();
        }
    }
}