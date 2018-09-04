using System;

namespace ScrumBoard.Models {

  public abstract class BaseEntity {
  
    public DateTime createdAt {get;set;}
    public DateTime updatedAt {get;set;}

  }

}