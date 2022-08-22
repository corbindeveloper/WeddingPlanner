#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingId { get; set; }


    // WEDDER ONE
    [Required(ErrorMessage = "is required")]
    [Display(Name = "Wedder One:")]
    public string WedderOne { get; set; }

    // WEDDER TWO
    [Required(ErrorMessage = "is required")]
    [Display(Name = "Wedder Two:")]
    public string WedderTwo { get; set; }

    // DATE
    [Required(ErrorMessage = "is required")]
    [Display(Name = "Wedding Date:")]
    public DateTime Date { get; set; }

    // ADDRESS
    [Required(ErrorMessage = "is required")]
    [Display(Name = "Wedding Address:")]
    public string? Address { get; set; }

    // CREATED && UPDATED
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // USER
    public int? UserId { get; set; }
    public User? Creator { get; set; }

    // CONNECTION
    public List<Connection> Connections { get; set; } = new List<Connection>();

    // COUPLE
    public string Couple()
    {
        return WedderOne + " " + WedderTwo;
    }
    


}