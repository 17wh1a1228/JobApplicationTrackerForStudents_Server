using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerForStudents_Server.Models;

public partial class Application
{
    [Key]
    public int JobId { get; set; }

    [StringLength(50)]
    public string Position { get; set; } = null!;

    [StringLength(50)]
    public string Company { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    public int StatusId { get; set; }

    [ForeignKey("Student")]
    public int StudentId { get; set; }
    public virtual Student Student { get; set; }
}
