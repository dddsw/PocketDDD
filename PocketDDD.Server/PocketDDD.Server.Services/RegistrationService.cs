﻿using PocketDDD.Server.DB;
using PocketDDD.Server.Model.DBModel;
using PocketDDD.Server.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PocketDDD.Shared.API.RequestDTOs;
using PocketDDD.Shared.API.ResponseDTOs;

namespace PocketDDD.Server.Services;
public class RegistrationService
{
    private readonly PocketDDDContext dbContext;

    public RegistrationService(PocketDDDContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<LoginResultDTO> Register(RegisterDTO dto)
    {
        var user = new User
        {
            EventDetailId = 1,
            Name = dto.Name,
            Token = GenerateBearerToken(),
            EventScore = 1
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var dtoResponse = new LoginResultDTO(user.Name, user.Token);

        return dtoResponse;
    }

    private string GenerateBearerToken()
    {
        var characterArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Distinct().ToArray();
        const int length = 40;
        var token = new char[length];

        using (var cryptRNG = RandomNumberGenerator.Create())
        {
            byte[] tokenBuffer = new byte[length * 8];

            cryptRNG.GetBytes(tokenBuffer);

            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(tokenBuffer, i * 8);
                token[i] = characterArray[value % (uint)characterArray.Length];
            }
        }

        return new string(token);
    }
}
