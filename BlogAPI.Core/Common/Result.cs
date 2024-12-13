﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAPI.Core.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new();

        public static Result<T> Success(T data, string message = null) =>
            new() { IsSuccess = true, Data = data, Message = message };

        public static Result<T> Failure(string message, List<string> errors = null) =>
            new() { IsSuccess = false, Message = message, Errors = errors ?? new List<string>() };
    }
}
