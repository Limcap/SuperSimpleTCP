using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Limcap.SSTcp {
	public class SSTcpClient : SSTcpEntity {
		public SSTcpClient( string ip, int port )
		: base( ip, port ) { }








		public string Send( string content ) {
			var stepper = new ByteArrayStepper( Encoding.UTF8.GetBytes( content ) );
			var client = new TcpClient( ip.ToString(), port );
			var responseParts = new List<string>();

			using (NetworkStream nwStream = client.GetStream()) {

				// Começa o envio dos dados do content.
				Console.WriteLine( "SSTcpClient: Enviando dados..." );

				// Envia primeiro a quantidade de bytes a ser enviados
				nwStream.Write( BitConverter.GetBytes( stepper.array.Length ), 0, 4 );

				// Envia os dados.
				while (stepper.NextStep())
					nwStream.Write( stepper.array, stepper.StepIndex, stepper.StepSize );
				Console.WriteLine( "SSTcpClient: Dados enviados." );

				// Inicializa as variáveis para recebimento de resposta.
				byte[] buffer = new byte[ByteArrayStepper.StepMaxSize];
				bool completed = false;
				int amountOfDataToRead = -1;
				int totalBytesRead = 0;
				Console.WriteLine( "SSTcpClient: Recebendo resposta...." );

				// Espera até 5 segundos entre dados ficarem disponíveis na stream.
				for (int attempt = 0; attempt < 50; attempt++) {
					if (!nwStream.DataAvailable) {
						Thread.Sleep( 100 );
					}
					else {
						attempt = 0;

						// Na primeira iteração, lê o total de bytes a ser lido
						if (amountOfDataToRead == -1) {
							totalBytesRead = nwStream.Read( buffer, 0, 4 );
							amountOfDataToRead = BitConverter.ToInt32( buffer, 0 );
						}

						// A partir da segunda iteração, começa a fazer a leitura
						else {
							int bytesRead = nwStream.Read( buffer, 0, ByteArrayStepper.StepMaxSize );
							totalBytesRead += bytesRead;
							string receivedString = Encoding.UTF8.GetString( buffer, 0, bytesRead );
							responseParts.Add( receivedString );
						}
					}

					// Após a leitura, verifica se já leu o total, se sim, sai.
					if (totalBytesRead >= amountOfDataToRead && amountOfDataToRead > -1) {
						completed = true;
						break;
					}
				}

				// Avisa se a resposta foi recebida completamente.
				if (completed) {
					Console.WriteLine( "SSTcpClient: Resposta recebida." );
				}
				else {
					Console.WriteLine( "SSTcpClient: Resposta incompleta." );
				}
			}

			client.Close();
			return string.Join( "", responseParts );
		}








		//public string Send_Old( string content )
		//{
		//	var packets = GetPacketsFromContent( content );
		//	var client = new TcpClient( ip.ToString(), port );

		//	using( NetworkStream nwStream = client.GetStream() ) {

		//		// Evia os bytes
		//		Console.WriteLine( "SSTcpClient: Enviando dados, " + packets.Count + " Packets..." );
		//		foreach( var packet in packets ) {
		//			nwStream.Write( packet, 0, packet.Length );
		//			if( nwStream.Length > 0 )
		//				Console.WriteLine( "SSTcpClient: Esperando Packet ser consumido da Stream" );
		//			while( nwStream.Length > 0 )
		//				Thread.Sleep( 100 );
		//			Console.WriteLine( "SSTcpClient: Pacote consumido." );
		//		}
		//		Console.WriteLine( "SSTcpClient: Envio concluido" );

		//		// Recebe resposta
		//		byte[] responseBytes = new byte[SSTcpEntity.packetLength];
		//		var contentParts = new List<string>();
		//		bool completed = false;

		//		Console.WriteLine( "SSTcpClient: Recebendo resposta...." );
		//		for( int attempt = 0; attempt < 50; attempt++ ) {
		//			if( !nwStream.DataAvailable ) {
		//				Thread.Sleep( 100 );
		//			}
		//			else {
		//				Console.WriteLine( "SSTcpClient: Packet recebido." );
		//				attempt = 0;
		//				int bytesRead = nwStream.Read( responseBytes, 0, SSTcpEntity.packetLength );

		//				var partIndex_Bytes = new byte[4] { 0, 0, responseBytes[0], responseBytes[1] };
		//				if( BitConverter.IsLittleEndian ) Array.Reverse( partIndex_Bytes );
		//				int partIndex = BitConverter.ToInt32( partIndex_Bytes, 0 );

		//				var qntParts_Bytes = new byte[4] { 0, 0, responseBytes[2], responseBytes[3] };
		//				if( BitConverter.IsLittleEndian ) Array.Reverse( qntParts_Bytes );
		//				int qntParts = BitConverter.ToInt32( qntParts_Bytes, 0 );

		//				string contentBytes = Encoding.UTF8.GetString( responseBytes, 4, bytesRead - 4 );
		//				contentParts.Add( contentBytes );

		//				if( partIndex == qntParts ) {
		//					completed = true;
		//					break;
		//				}
		//			}
		//		}

		//		if( completed ) {
		//			Console.WriteLine( "SSTcpClient: Resposta recebida." );
		//		}
		//		else {
		//			Console.WriteLine( "SSTcpClient: Resposta incompleta." );
		//		}

		//		return string.Join( "", contentParts );
		//	}
		//}
	}
}
