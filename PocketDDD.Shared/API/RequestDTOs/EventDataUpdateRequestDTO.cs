﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketDDD.Shared.API.RequestDTOs;
public record class EventDataUpdateRequestDTO
{
    public int Version { get; set; }
}
