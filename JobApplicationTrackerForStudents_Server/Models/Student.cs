using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerForStudents_Server.Models;

public partial class Student
{
    [Key]
    public int StudentId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; }

    [DataType(DataType.EmailAddress)]
    [StringLength(200)]
    public string Email { get; set; } = null!;

    [DataType(DataType.PhoneNumber)]
    [StringLength(20)]
    public string Phone { get; set; }

    [DataType(DataType.Url)]
    [StringLength(200)]
    public string Url { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

}