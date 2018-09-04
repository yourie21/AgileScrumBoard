using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; //added for date validate

namespace ScrumBoard.Models{

  public class Scrum : BaseEntity {
    [Key]
    public int id { get; set;}

    [Required(ErrorMessage="Title is required.")]
    public string title { get; set;}

    [Required(ErrorMessage="Description of this task is required.")]
    public string dx { get; set;}

    [Required(ErrorMessage="Start date is required.")]
    public DateTime startdate { get; set;}

    [Required(ErrorMessage="End date is required, and must be later than the start date and today's date.")]
    public DateTime enddate { get; set;}

    [Required(ErrorMessage="Status must be chosen.")]
    public string status { get; set;}

    [Required(ErrorMessage="Responsible department should be entered.")]
    public string dept { get; set;}

    public int UserId { get; set;}
    public sqlUser user { get; set;}
    
    public List<Participant> parts { get; set;}
    public Scrum() {
      parts = new List<Participant>();
      createdAt = DateTime.Now;
      updatedAt = DateTime.Now;
    }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
      if (enddate < DateTime.Now){
        yield return new ValidationResult(
          "End Date must be in the future!"
        );
      }
    }
  }
}