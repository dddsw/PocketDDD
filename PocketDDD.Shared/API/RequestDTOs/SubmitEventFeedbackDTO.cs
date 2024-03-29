﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketDDD.Shared.API.RequestDTOs;
public record SubmitEventFeedbackDTO
{
    public string ClientId { get; init; } = Guid.NewGuid().ToString();
    public DateTimeOffset CreatedOn { get; init; }
    public int RefreshmentsRating { get; init; }
    public int VenueRating { get; init; }
    public int OverallRating { get; init; }
    public string Comments { get; init; } = string.Empty;
}
