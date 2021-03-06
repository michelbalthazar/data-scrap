﻿using DataTech.Domain.Common;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataTech.Infrastructure
{
    public static class HttpHelper<T>
    {
        public static async Task<Result<T>> ResponseValidate(HttpResponseMessage response)
        {
            var status = response?.StatusCode;

            if (status == HttpStatusCode.BadRequest)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return new Result<T>(ResultStatusCode.BadRequest, responseData);
            }
            else
            if (status == HttpStatusCode.NotFound)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return new Result<T>(ResultStatusCode.NotFound, responseData);
            }
            else
            if (status == HttpStatusCode.Unauthorized)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return new Result<T>(ResultStatusCode.Unauthorized, responseData);
            }
            else
            if (status == HttpStatusCode.RequestTimeout)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return new Result<T>(ResultStatusCode.TimedOut, responseData ?? "timeout");
            }
            else
            if (status == HttpStatusCode.Conflict)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return new Result<T>(ResultStatusCode.Duplicated, responseData);
            }
            else
            if (status == HttpStatusCode.InternalServerError)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return new Result<T>(ResultStatusCode.Error, responseData);
            }
            else
            if (status != HttpStatusCode.Accepted && status != HttpStatusCode.OK && status != HttpStatusCode.NoContent && status != HttpStatusCode.Created)
            {
                var responseData = await response?.Content.ReadAsStringAsync();
                return new Result<T>(ResultStatusCode.Error, responseData ?? "The HTTP status code of the response was not expected");
            }

            return new Result<T>(ResultStatusCode.OK, response);
        }

        public static async Task<Result<T>> ResponseReadAsJsonStringAsync(HttpResponseMessage response)
        {
            var validate = await ResponseValidate(response);

            if (validate.Status != ResultStatusCode.OK)
                return validate;

            var responseResult = await response.Content.ReadAsStringAsync();

            return string.IsNullOrWhiteSpace(responseResult) == false
                ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseResult)
                : default(T);
        }

        public static async Task<Result<T>> ResponseReadAsStringAsync(HttpResponseMessage response, bool ignoreRootElement)
        {
            var validate = await ResponseValidate(response);

            if (validate.Status != ResultStatusCode.OK)
                return validate;

            var responseResult = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseResult))
                return default(T);

            return ignoreRootElement
            ? JObject.Parse(responseResult).Properties().First().Value.ToObject<T>()
            : JObject.Parse(responseResult).ToObject<T>();
        }

        public static async Task<Result<T>> ResponseReadAsStringAsync(HttpResponseMessage response)
        {
            var validate = await ResponseValidate(response);

            if (validate.Status != ResultStatusCode.OK)
                return validate;

            var responseResult = await response.Content.ReadAsStringAsync();

            return new Result<T>(ResultStatusCode.OK, responseResult);
        }

        public static async Task<Result<T>> ResponseReadAsByteAsync(HttpResponseMessage response)
        {
            var validate = await ResponseValidate(response);

            if (validate.Status != ResultStatusCode.OK)
                return validate;

            var result = await response.Content.ReadAsByteArrayAsync();

            return new Result<T>(ResultStatusCode.OK, result);
        }
    }
}