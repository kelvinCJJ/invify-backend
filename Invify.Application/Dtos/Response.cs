﻿namespace Invify.Dtos
{
    public class Response
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = null;
        public object? Value { get; set; } = null;
        public IEnumerable<string> Errors { get; set; }

    }
}
