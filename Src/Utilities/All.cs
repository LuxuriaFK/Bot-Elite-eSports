/*
 * Arquivo: All.cs
 * Criado em: 10-6-2021
 * https://github.com/ForceFK
 * ForceFK - Force&Kuraiyo Dev
 * Última modificação: 10-6-2021
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BOTDiscord.Utilities
{
    public static class All
    { 
        /// <summary>
      /// Retorna uma % entre 2 valores
      /// </summary>
      /// <param name="current">Valor atual</param>
      /// <param name="total">Valor total (100%)</param>
      /// <returns></returns>
        public static decimal GetRatio(long current, long total)
        {
            decimal result;
            if (current <= 0)
                result = 0;
            else if (current + 1 >= total)
                result = 100;
            else
            {
                result = Math.Round(current / (decimal)total * 100, 1);
                result = result > 100 ? 100 : result;
            }
            return result;
        }

        /// <summary>
        /// Baixa um arquivo de uma URL
        /// </summary>
        /// <param name="url">Link do arquivo</param>
        /// <param name="path">Local onde será salvo</param>
        /// <returns></returns>
        public static string DownloadFile(string url, string path)
        {
            using var wc = new WebClient();
            wc.DownloadFile(url, path);

            return path;
        }

        /// <summary>
        /// Baixa textos de uma URL
        /// </summary>
        /// <param name="url">Caminho do texto a ser baixado</param>
        /// <returns></returns>
        public static string DownloadString(string url)
        {
            using var wc = new WebClient();
            return wc.DownloadString(url);
        }

        /// <summary>
        /// Verifica se a URI é valida
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static bool IsValidUrl(string query)
        {
            return Uri.TryCreate(query, UriKind.Absolute, out var uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeFile || uriResult.Scheme == Uri.UriSchemeFtp ||
                    uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps ||
                    uriResult.Scheme == Uri.UriSchemeNetTcp);
        }
    }
}
