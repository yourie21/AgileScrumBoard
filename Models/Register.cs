using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumBoard.Models{
    public class Userlogin { //Register class is the database!!!
        [Key]
        public int id {get; set;}

        [Required (ErrorMessage = "Please enter your name")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters.")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage="Name must have letters only")]
        public string name {get; set;}

        [Required (ErrorMessage = "Please enter last name name")]
        [MinLength(3, ErrorMessage = "Last name must be at least 2 characters.")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage="Name must have letters only")]
        public string lastname {get; set;}

        [Required (ErrorMessage = "Please enter your email")]
        [EmailAddress]
        public string email {get; set;}

        [Required (ErrorMessage = "Please enter a password")]
        [MinLength(8, ErrorMessage = "Password must be at peast 8 characters.")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage="Password must contain at least one Uppercase letter (A-Z) one Lowercase letter (a-z) one Digit (0-9) and a Special character (#?!@$%^&*-).")]
        [DataType (DataType.Password)] 
        public string pw {get; set;}

        [Required (ErrorMessage = "Password confirmation is required.")]
        [DataType(DataType.Password)]
        [Compare("pw", ErrorMessage="Passwords must match!")]
        public string cw {get; set;}

        [Required (ErrorMessage = "Please choose your operation level.")]
        public int level {get; set;}

    }      
}