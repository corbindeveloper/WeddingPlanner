#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;

public class Connection
{
    [Key]
    public int ConnectionId { get; set; }

    // USER
    public int UserId { get; set; }
    public User? User { get; set; }

    // WEDDING
    public int WeddingId { get; set; }
    public Wedding? Wedding { get; set;}

    // CREATED && UPDATED
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

}