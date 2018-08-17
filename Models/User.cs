using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginRegistration.Models
{
    public class User{
        
        [Key]
        public int UserId {get;set;}
        
        [Display (Name = "First Name:")]
        [DataType(DataType.Text)]
        [Required]
        [MinLength(2)]
        public string first_name {get; set;}

        [Display (Name = "Last Name:")]
        [DataType(DataType.Text)]
        [Required]
        [MinLength(2)]
        public string last_name {get; set;}

        [Display (Name = "Email:")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string email {get; set;}

        [Display (Name = "Password:")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=\D*\d)(?=.*?[a-zA-Z]).*[\W_].*$" , 
         ErrorMessage = "Must have at least 1 letter, 1 number and 1 special character in password!")]
        
        [Required]
        [MinLength(8)]
        public string password {get; set;}

        [Display (Name = "Comfirm Password:")]
        [Required]
        [Compare("password", ErrorMessage = "Incorrect password confirmation!")]
        public string confirm {get; set;}
        
        public DateTime created_at {get; set;}
        public DateTime updated_at {get; set;}
        public List<Participant> participating {get;set;}
        public User()
        {
            participating = new List<Participant>();
        }
    }
    
}