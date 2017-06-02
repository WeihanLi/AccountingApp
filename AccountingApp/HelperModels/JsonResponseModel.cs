﻿namespace AccountingApp.HelperModels
{
    public class JsonResultModel
    {
        public JsonResultStatus Status { get; set; }

        public string Msg { get; set; }

        public string Data { get; set; }
    }

    public class JsonResultModel<T>
    {
        public JsonResultStatus Status { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }
    }

    public enum JsonResultStatus
    {
        Success = 200,
        ProcessFail = 500,
        NoPermission = 403,
        RequestError = 401,
        ResourceNotFound = 404
    }
}