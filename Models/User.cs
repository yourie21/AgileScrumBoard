using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // added

using System.Linq;
using System.Threading.Tasks;

namespace ScrumBoard.Models {

  public class sqlUser {
    [Key]
    public int id {get; set;}
    public string name { get; set;}
    public string lastname { get; set;}
    public string email { get; set;}
    public string pw { get; set;}
    public int level {get; set;}
    public List<Participant> parts  {get;set;}
    public List<Scrum> scrums  {get;set;}
    public sqlUser() {
      parts  = new List<Participant>();
      scrums  = new List<Scrum>();
    }
  }
}