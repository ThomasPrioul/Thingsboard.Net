﻿using System;
using Thingsboard.Net.Models;

namespace Thingsboard.Net.Exceptions;

public class TbHttpException : TbException
{
    public TbHttpException(TbResponseFault error) : base(error.Message ?? "")
    {
        StatusCode = error.Status;
        Timestamp  = error.Timestamp;
        ErrorCode  = error.ErrorCode;
    }

    public int ErrorCode { get; }

    public DateTime Timestamp { get; }

    public int StatusCode { get; }
}