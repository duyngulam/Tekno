using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tekno.Application.Catalog.DTOs.Admin
{
    /// <summary>
    /// Attribute information for a category
    /// </summary>
    public class CategoryAttributeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string InputType { get; set; } = "select"; // select, text, number
        public bool IsGlobal { get; set; } = false;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<AttributeValueDto> Values { get; set; } = new();
    }

    /// <summary>
    /// Attribute value information
    /// </summary>
    public class AttributeValueDto
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Create a new attribute for a category
    /// </summary>
    public class CreateAttributeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(select|text|number)$", ErrorMessage = "InputType must be 'select', 'text', or 'number'")]
        public string InputType { get; set; } = "select";

        public bool IsGlobal { get; set; } = false;

        public int? CategoryId { get; set; }

        /// <summary>
        /// Initial values for the attribute (only applicable for select type)
        /// </summary>
        public List<string>? InitialValues { get; set; }
    }

    /// <summary>
    /// Update an existing attribute
    /// </summary>
    public class UpdateAttributeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(select|text|number)$", ErrorMessage = "InputType must be 'select', 'text', or 'number'")]
        public string InputType { get; set; } = "select";
    }

    /// <summary>
    /// Add a new value to an attribute
    /// </summary>
    public class AddAttributeValueDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int AttributeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Update an attribute value
    /// </summary>
    public class UpdateAttributeValueDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ValueId { get; set; }

        [Required]
        [StringLength(100)]
        public string Value { get; set; } = string.Empty;
    }
}
