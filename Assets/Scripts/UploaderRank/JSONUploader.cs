using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JSONUploader : MonoBehaviour
{
    // URL do servidor para onde você enviará o JSON
    public string serverURL = "http://leilamd.com.br/backend/main.php"; // Note que adicionei "http://" ao URL

    // Método chamado quando o botão é pressionado
    public void UploadJSON()
    {
        StartCoroutine(UploadJSONCoroutine());
    }

    IEnumerator UploadJSONCoroutine()
    {
        // Caminho completo do arquivo JSON
        string jsonFilePath = Path.Combine(Application.persistentDataPath, "ranking.json"); // Use persistentDataPath para Android

        // Verifica se o arquivo JSON existe
        if (File.Exists(jsonFilePath))
        {
            // Lê o conteúdo do arquivo JSON
            string jsonContent = File.ReadAllText(jsonFilePath);

            // Cria uma requisição HTTP POST
            UnityWebRequest request = UnityWebRequest.Post(serverURL, "");

            // Define o corpo da requisição com os parâmetros e o arquivo JSON
            string boundary = "boundary";
            byte[] boundaryBytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            string body = "--" + boundary + "\r\n" +
                          "Content-Disposition: form-data; name=\"REQUEST_METHOD\"\r\n\r\n" +
                          "POST\r\n" +
                          "--" + boundary + "\r\n" +
                          "Content-Disposition: form-data; name=\"action\"\r\n\r\n" +
                          "upload-ranking\r\n" +
                          "--" + boundary + "\r\n" +
                          "Content-Disposition: form-data; name=\"file\"; filename=\"ranking.json\"\r\n" +
                          "Content-Type: application/json\r\n\r\n" +
                          jsonContent + "\r\n" +
                          "--" + boundary + "--\r\n";
            byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(body);

            // Configura a requisição
            UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(bodyBytes);
            uploadHandlerRaw.contentType = "multipart/form-data; boundary=" + boundary;
            request.uploadHandler = uploadHandlerRaw;
            request.downloadHandler = new DownloadHandlerBuffer();

            // Defina o tempo limite em segundos (por exemplo, 30 segundos)
            request.timeout = 30;

            Debug.Log("Cabeçalhos da solicitação: " + request.GetRequestHeader("Content-Type"));

            // Envia a requisição para o servidor
            yield return request.SendWebRequest();

            Debug.Log("isNetworkError:" + request.isNetworkError + " | isHttpError: " + request.isHttpError);

            // Verifica se houve algum erro durante o envio
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Erro ao enviar o JSON para o servidor: " + request.error);
                // Log do retorno do servidor em caso de erro
                Debug.LogError("Resposta do servidor: " + request.downloadHandler.text);
            }
            else
            {
                Debug.Log("JSON enviado com sucesso para o servidor.");
            }
        }
        else
        {
            Debug.LogError("Arquivo JSON não encontrado em: " + jsonFilePath);
        }
    }
}
