using AlephNote.PluginInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace AlephNote.Repository
{
	public class SimpleJsonRest : ISimpleJsonRest
	{
		private const int LOG_FMT_DEPTH = 3;

		private readonly HttpClient _client;
		private readonly Uri _host;
		private readonly IAlephLogger _logger;

		private JsonConverter[] _converter = new JsonConverter[0];
		private StringEscapeHandling _seHandling = StringEscapeHandling.Default;
		private Tuple<string, string> _urlAuthentication = null;
		private Dictionary<string, string> _headers = new Dictionary<string, string>();
		private HttpResponseMessage _lastResponse = null;

		public SimpleJsonRest(IWebProxy proxy, string host, IAlephLogger log)
		{
			if (proxy != null)
			{
				_client = new HttpClient(new HttpClientHandler {Proxy = proxy});
			}
			else
			{
				_client = new HttpClient();
			}

			_headers["User-Agent"] = "AlephNote/Common";
			_headers["ContentType"] = "application/json";

			_host = new Uri(host);

			_logger = log;
		}

		public void Dispose()
		{
			_client?.Dispose();
		}

		public void AddConverter(object ic)
		{
			var c = (JsonConverter) ic;

			_converter = _converter.Concat(new[] {c}).ToArray();
		}

		public void SetEscapeAllNonASCIICharacters(bool escape)
		{
			_seHandling = escape ? StringEscapeHandling.EscapeNonAscii : StringEscapeHandling.Default;
		}

		public void SetURLAuthentication(string username, string password)
		{
			_urlAuthentication = Tuple.Create(username, password);
		}

		private Uri CreateUri(string path, params string[] parameter)
		{
			var uri = new Uri(_host, path);

			var result = uri.ToString();

			if (_urlAuthentication != null)
			{
				result = string.Format("{0}://{1}:{2}@{3}:{4}{5}",
					uri.Scheme,
					Uri.EscapeDataString(_urlAuthentication.Item1),
					Uri.EscapeDataString(_urlAuthentication.Item2),
					uri.Host,
					uri.Port,
					uri.AbsolutePath);
			}

			bool first = true;
			foreach (var param in parameter)
			{
				if (first)
					result += "?" + param;
				else
					result += "&" + param;

				first = false;
			}

			return new Uri(result);
		}

		public void AddHeader(string name, string value)
		{
			_headers[name] = value;
		}

		public string GetResponseHeader(string name)
		{
			return string.Join("\n", _lastResponse.Headers.GetValues(name));
		}

		private JsonSerializerSettings GetSerializerSettings()
		{
			return new JsonSerializerSettings
			{
				Converters = _converter,
				StringEscapeHandling = _seHandling,
			};
		}

		#region POST

		public TResult PostTwoWay<TResult>(object body, string path, params string[] parameter)
		{
			return GenericTwoWay<TResult>(body, path, HttpMethod.Post, new int[0], parameter);
		}

		public TResult PostTwoWay<TResult>(object body, string path, int[] allowedStatusCodes, params string[] parameter)
		{
			return GenericTwoWay<TResult>(body, path, HttpMethod.Post, allowedStatusCodes, parameter);
		}

		public void PostUpload(object body, string path, params string[] parameter)
		{
			GenericUpload(body, path, HttpMethod.Post, new int[0], parameter);
		}

		public void PostUpload(object body, string path, int[] allowedStatusCodes, params string[] parameter)
		{
			GenericUpload(body, path, HttpMethod.Post, allowedStatusCodes, parameter);
		}

		public TResult PostDownload<TResult>(string path, params string[] parameter)
		{
			return GenericDownload<TResult>(path, HttpMethod.Post, new int[0], parameter);
		}

		public TResult PostDownload<TResult>(string path, int[] allowedStatusCodes, params string[] parameter)
		{
			return GenericDownload<TResult>(path, HttpMethod.Post, allowedStatusCodes, parameter);
		}

		public void PostEmpty(string path, params string[] parameter)
		{
			GenericEmpty(path, HttpMethod.Post, new int[0], parameter);
		}

		public void PostEmpty(string path, int[] allowedStatusCodes, params string[] parameter)
		{
			GenericEmpty(path, HttpMethod.Post, allowedStatusCodes, parameter);
		}

		#endregion

		#region PUT

		public TResult PutTwoWay<TResult>(object body, string path, params string[] parameter)
		{
			return GenericTwoWay<TResult>(body, path, HttpMethod.Put, new int[0], parameter);
		}

		public TResult PutTwoWay<TResult>(object body, string path, int[] allowedStatusCodes, params string[] parameter)
		{
			return GenericTwoWay<TResult>(body, path, HttpMethod.Put, allowedStatusCodes, parameter);
		}

		public void PutUpload(object body, string path, params string[] parameter)
		{
			GenericUpload(body, path, HttpMethod.Put, new int[0], parameter);
		}

		public void PutUpload(object body, string path, int[] allowedStatusCodes, params string[] parameter)
		{
			GenericUpload(body, path, HttpMethod.Put, allowedStatusCodes, parameter);
		}

		public TResult PutDownload<TResult>(string path, params string[] parameter)
		{
			return GenericDownload<TResult>(path, HttpMethod.Put, new int[0], parameter);
		}

		public TResult PutDownload<TResult>(string path, int[] allowedStatusCodes, params string[] parameter)
		{
			return GenericDownload<TResult>(path, HttpMethod.Put, allowedStatusCodes, parameter);
		}

		public void PutEmpty(string path, params string[] parameter)
		{
			GenericEmpty(path, HttpMethod.Put, new int[0], parameter);
		}

		public void PutEmpty(string path, int[] allowedStatusCodes, params string[] parameter)
		{
			GenericEmpty(path, HttpMethod.Put, allowedStatusCodes, parameter);
		}

		#endregion

		#region DELETE

		public TResult DeleteTwoWay<TResult>(object body, string path, params string[] parameter)
		{
			return GenericTwoWay<TResult>(body, path, HttpMethod.Delete, new int[0], parameter);
		}

		public TResult DeleteTwoWay<TResult>(object body, string path, int[] allowedStatusCodes, params string[] parameter)
		{
			return GenericTwoWay<TResult>(body, path, HttpMethod.Delete, allowedStatusCodes, parameter);
		}

		public void DeleteUpload(object body, string path, params string[] parameter)
		{
			GenericUpload(body, path, HttpMethod.Delete, new int[0], parameter);
		}

		public void DeleteUpload(object body, string path, int[] allowedStatusCodes, params string[] parameter)
		{
			GenericUpload(body, path, HttpMethod.Delete, allowedStatusCodes, parameter);
		}

		public TResult DeleteDownload<TResult>(string path, params string[] parameter)
		{
			return GenericDownload<TResult>(path, HttpMethod.Delete, new int[0], parameter);
		}

		public TResult DeleteDownload<TResult>(string path, int[] allowedStatusCodes, params string[] parameter)
		{
			return GenericDownload<TResult>(path, HttpMethod.Delete, allowedStatusCodes, parameter);
		}

		public void DeleteEmpty(string path, params string[] parameter)
		{
			GenericEmpty(path, HttpMethod.Delete, new int[0], parameter);
		}

		public void DeleteEmpty(string path, int[] allowedStatusCodes, params string[] parameter)
		{
			GenericEmpty(path, HttpMethod.Delete, allowedStatusCodes, parameter);
		}

		#endregion

		#region GET

		public TResult Get<TResult>(string path, params string[] parameter)
		{
			return Get<TResult>(path, new int[0], parameter);
		}

		public TResult Get<TResult>(string path, int[] allowedStatusCodes, params string[] parameter)
		{
			return GenericDownload<TResult>(path, HttpMethod.Get, allowedStatusCodes, parameter);
		}

		#endregion

		#region GENERIC

		private TResult GenericTwoWay<TResult>(object body, string path, HttpMethod method, int[] allowedStatusCodes, params string[] parameter)
		{
			var uri = CreateUri(path, parameter);
			
			string upload = JsonConvert.SerializeObject(body, GetSerializerSettings());
			string download;
			HttpResponseMessage resp;
			try
			{
				var request = new HttpRequestMessage
				{
					Content = new StringContent(upload),
					RequestUri = uri,
					Method = method,
					
				};
				_headers.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));
				
				resp = _client.SendAsync(request).Result;

				if (!resp.IsSuccessStatusCode)
				{
					if (allowedStatusCodes.Any(sc => sc == (int)resp.StatusCode))
					{
						_logger.Debug("REST", string.Format("REST call to '{0}' [{3}] returned (allowed) statuscode {1} ({2})", uri, (int)resp.StatusCode, resp.StatusCode, method));
						_lastResponse = resp;

						return default(TResult);
					}

					throw new RestException("Server " + uri.Host + " returned status code: " + resp.StatusCode + " : " + resp.ReasonPhrase);
				}

				download = resp.Content.ReadAsStringAsync().Result;
			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.Count == 1)
					throw new RestException("Could not communicate with server " + uri.Host, e.InnerExceptions.First());
				else
					throw new RestException("Could not communicate with server " + uri.Host, e);
			}
			catch (Exception e)
			{
				throw new RestException("Could not communicate with server " + uri.Host, e);
			}

			TResult downloadObject;
			try
			{
				downloadObject = JsonConvert.DeserializeObject<TResult>(download, _converter);
			}
			catch (Exception e)
			{
				throw new RestException("Rest call to " + uri.Host + " returned unexpected data :\r\n" + download, e);
			}

			_logger.Debug("REST",
				string.Format("Calling REST API '{0}' [{1}]", uri, method),
				string.Format("Send:\r\n{0}\r\n\r\n---------------------\r\n\r\nRecieved:\r\n{1}",
				CompactJsonFormatter.FormatJSON(upload, LOG_FMT_DEPTH),
				CompactJsonFormatter.FormatJSON(download, LOG_FMT_DEPTH)));

			_lastResponse = resp;
			return downloadObject;
		}

		private void GenericUpload(object body, string path, HttpMethod method, int[] allowedStatusCodes, params string[] parameter)
		{
			var uri = CreateUri(path, parameter);

			string upload = JsonConvert.SerializeObject(body, GetSerializerSettings());
			HttpResponseMessage resp;

			try
			{
				var request = new HttpRequestMessage
				{
					Content = new StringContent(upload),
					RequestUri = uri,
					Method = method,
				};
				_headers.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));

				resp = _client.SendAsync(request).Result;

				if (!resp.IsSuccessStatusCode)
				{
					if (allowedStatusCodes.Any(sc => sc == (int)resp.StatusCode))
					{
						_logger.Debug("REST", string.Format("REST call to '{0}' [{3}] returned (allowed) statuscode {1} ({2})", uri, (int)resp.StatusCode, resp.StatusCode, method));
						_lastResponse = resp;

						return;
					}

					throw new RestException("Server " + uri.Host + " returned status code: " + resp.StatusCode + " : " + resp.ReasonPhrase);
				}
			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.Count == 1)
					throw new RestException("Could not communicate with server " + uri.Host, e.InnerExceptions.First());
				else
					throw new RestException("Could not communicate with server " + uri.Host, e);
			}
			catch (Exception e)
			{
				throw new RestException("Could not communicate with server " + uri.Host, e);
			}

			_logger.Debug("REST",
				string.Format("Calling REST API '{0}' [{1}]", uri, method),
				string.Format("Send:\r\n{0}\r\n\r\nRecieved: Nothing",
				CompactJsonFormatter.FormatJSON(upload, LOG_FMT_DEPTH)));

			_lastResponse = resp;
		}

		private TResult GenericDownload<TResult>(string path, HttpMethod method, int[] allowedStatusCodes, params string[] parameter)
		{
			var uri = CreateUri(path, parameter);

			string download;
			HttpResponseMessage resp;

			try
			{
				var request = new HttpRequestMessage
				{
					RequestUri = uri,
					Method = method,
				};
				_headers.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));

				resp = _client.SendAsync(request).Result;

				if (!resp.IsSuccessStatusCode)
				{
					if (allowedStatusCodes.Any(sc => sc == (int)resp.StatusCode))
					{
						_logger.Debug("REST", string.Format("REST call to '{0}' [{3}] returned (allowed) statuscode {1} ({2})", uri, (int)resp.StatusCode, resp.StatusCode, method));
						_lastResponse = resp;

						return default(TResult);
					}

					throw new RestException("Server " + uri.Host + " returned status code: " + resp.StatusCode + " : " + resp.ReasonPhrase);
				}

				download = resp.Content.ReadAsStringAsync().Result;
			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.Count == 1)
					throw new RestException("Could not communicate with server " + uri.Host, e.InnerExceptions.First());
				else
					throw new RestException("Could not communicate with server " + uri.Host, e);
			}
			catch (Exception e)
			{
				throw new RestException("Could not communicate with server " + uri.Host, e);
			}

			TResult downloadObject;
			try
			{
				downloadObject = JsonConvert.DeserializeObject<TResult>(download, _converter);
			}
			catch (Exception e)
			{
				throw new RestException("Rest call to " + uri.Host + " returned unexpected data", "Rest call to " + uri.Host + " returned unexpected data :\r\n" + download, e);
			}
			
			_logger.Debug("REST",
				string.Format("Calling REST API '{0}' [{1}]", uri, method),
				string.Format("Send: Nothing\r\nRecieved:\r\n{0}",
				CompactJsonFormatter.FormatJSON(download, LOG_FMT_DEPTH)));

			_lastResponse = resp;
			return downloadObject;
		}

		private void GenericEmpty(string path, HttpMethod method, int[] allowedStatusCodes, params string[] parameter)
		{
			var uri = CreateUri(path, parameter);

			HttpResponseMessage resp;

			try
			{
				var request = new HttpRequestMessage
				{
					RequestUri = uri,
					Method = method,
				};
				_headers.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));

				resp = _client.SendAsync(request).Result;

				if (!resp.IsSuccessStatusCode)
				{
					if (allowedStatusCodes.Any(sc => sc == (int)resp.StatusCode))
					{
						_logger.Debug("REST", string.Format("REST call to '{0}' [{3}] returned (allowed) statuscode {1} ({2})", uri, (int)resp.StatusCode, resp.StatusCode, method));
						_lastResponse = resp;

						return;
					}

					throw new RestException("Server " + uri.Host + " returned status code: " + resp.StatusCode + " : " + resp.ReasonPhrase);
				}
			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.Count == 1)
					throw new RestException("Could not communicate with server " + uri.Host, e.InnerExceptions.First());
				else
					throw new RestException("Could not communicate with server " + uri.Host, e);
			}
			catch (Exception e)
			{
				throw new RestException("Could not communicate with server " + uri.Host, e);
			}

			_logger.Debug("REST", string.Format("Calling REST API '{0}' [{1}]", uri, method), "Send: Nothing\r\n\r\nRecieved: Nothing");

			_lastResponse = resp;
		}

		#endregion
	}
}