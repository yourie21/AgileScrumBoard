using Microsoft.EntityFrameworkCore;
 
namespace ScrumBoard.Models
{
    public class ReviewContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public ReviewContext(DbContextOptions<ReviewContext> options) : base(options) { }
        public DbSet<sqlUser> usertable { get; set; } //this name "usertable" is the table name in SQL, also at homecontroller
        public DbSet<Scrum> scrumtable { get; set; } 
        public DbSet<Participant> partstable { get; set; } 
    }
}
