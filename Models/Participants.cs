using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ScrumBoard.Models{

  public class Participant : BaseEntity {
    [Key]
    public int id { get; set; }

    public int ScrumId { get; set; }
    public Scrum scrum { get; set; }

    public int UserId { get; set; }
    public sqlUser user { get; set; }

    public Participant() {
      createdAt = DateTime.Now;
      updatedAt = DateTime.Now;
    }

  }
}