using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginRegistration.Models
{
    public class Participant{
        
        [Key]
        public int ParticipantId {get;set;}
        public int userid {get;set;}
        public User user {get;set;}
        public int activityid{get;set;}
        public Activity activity {get;set;}
    }
}