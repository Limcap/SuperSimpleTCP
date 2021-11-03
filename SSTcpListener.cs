using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Limcap.SSTcp {
	public class SSTcpListener : SSTcpEntity {
		private TcpListener listener { get; set; }
		private Func<string, string> callback { get; set; }
		private bool isRunning { get; set; }








		public SSTcpListener( int port, Func<string, string> callback )
		: this( null, port, callback ) { }


		public SSTcpListener( string ip, int port, Func<string, string> callback )
		: base( ip, port ) {
			this.listener = new TcpListener( base.ip, port );
			this.callback = callback;
		}








		public void StopListening() {
			isRunning = false;
			listener.Stop();
		}








		public void Destroy() {
			listener.Server.Close();
		}








		public void Listen() {
			listener.Start();
			isRunning = true;
			while (isRunning) {
				Console.WriteLine( "SSTcpListener: Esperando conexão..." );
				try {
					TcpClient client = listener.AcceptTcpClient();
					Console.WriteLine( "SSTcpListener: Conexão estabelecida!" );
					HandleClient( client );
					Console.WriteLine( "SSTcpListener: Conexão finalizada!" );
				}
				catch (SocketException e) {
					//if( e.Message != "A blocking operation was interrupted by a call to WSACancelBlockingCall" )
					if (!e.Message.Contains( "WSACancelBlockingCall" ))
						Console.WriteLine( "Serviço finalizado na porta " + port + "\nErro:\n" + e.ToString(), "SSTcpListener" );
				}
			}
			listener.Stop();
		}








		private void HandleClient( TcpClient client ) {
			// Get the incoming data through a network stream
			using (NetworkStream nwStream = client.GetStream()) {

				// Inicializa as variáveis para recebimento.
				byte[] buffer = new byte[ByteArrayStepper.StepMaxSize];
				var contentBytes = new List<byte>(4000);
				bool completed = false;
				int amountOfDataToRead = -1;
				int totalBytesRead = 0;

				// Espera até 5 segundos para cada parte da stream ficar disponível
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
							//string contentBytes = Encoding.ASCII.GetString( buffer, 0, bytesRead );
							//contentParts.Add( contentBytes );
							contentBytes.AddRange( buffer );
						}
					}

					// Após a leitura, verifica se já leu o total, se sim, sai.
					if (totalBytesRead >= amountOfDataToRead && amountOfDataToRead > -1) {
						completed = true;
						break;
					}
				}

				// Avisa se os dados foram recebidos completamente.
				if (completed) {
					Console.WriteLine( "SSTcpListener: Dados recebidos OK." );
				}
				else {
					Console.WriteLine( "SSTcpClient: Dados recebidos incompletos." );
				}

				// Processa os dados recebidos.
				string message = Encoding.UTF8.GetString( contentBytes.ToArray(), 0, totalBytesRead ).Trim('\0');

				//string received = string.Join( "", contentBytes );
				System.IO.File.WriteAllText( "request.txt", message );
				Console.WriteLine( "SSTcpListener: Dados recebidos: " + message.Replace( "\n", "\\n" ).Replace( "\r", "" ) );
				string response = callback( message );
				Console.WriteLine( "SSTcpListener: Dados de resposta: " + response.Replace( "\n", "\\n" ).Replace( "\r", "" ) );

				// Começa o envio dos dados de resposta.
				var stepper = new ByteArrayStepper( Encoding.UTF8.GetBytes( response ) );

				// Envia primeiro a quantidade de bytes a ser enviados
				nwStream.Write( BitConverter.GetBytes( stepper.array.Length ), 0, 4 );

				// Envia os dados.
				while (stepper.NextStep())
					nwStream.Write( stepper.array, stepper.StepIndex, stepper.StepSize );

				Console.WriteLine( "SSTcpListener: Dados de resposta enviados." );
			}
			client.Close();
		}








		//private void HandleClient_Old( TcpClient client )
		//{
		//	// Get the incoming data through a network stream
		//	using( NetworkStream nwStream = client.GetStream() ) {
		//		byte[] buffer = new byte[SSTcpEntity.packetLength];

		//		var contentParts = new List<string>();
		//		bool completed = false;

		//		for( int i = 0; i < 50; i++ ) {
		//			if( !nwStream.DataAvailable )
		//				Thread.Sleep( 100 );
		//			else {
		//				Console.WriteLine( "SSTcpListener: Packet recebido." );
		//				i = 0;
		//				int bytesRead = nwStream.Read( buffer, 0, SSTcpEntity.packetLength );

		//				var partIndexBytes = new byte[4] { 0, 0, buffer[0], buffer[1] };
		//				if( BitConverter.IsLittleEndian ) Array.Reverse( partIndexBytes );
		//				int curPartIndex = BitConverter.ToInt32( partIndexBytes, 0 );

		//				var partsTotalBytes = new byte[4] { 0, 0, buffer[2], buffer[3] };
		//				if( BitConverter.IsLittleEndian ) Array.Reverse( partsTotalBytes );
		//				int endPartIndex = BitConverter.ToInt32( partsTotalBytes, 0 );

		//				string contentBytes = Encoding.UTF8.GetString( buffer, 4, bytesRead - 4 );
		//				contentParts.Add( contentBytes );

		//				if( curPartIndex == endPartIndex ) {
		//					completed = true;
		//					break;
		//				}
		//			}
		//		}

		//		if( completed ) {
		//			Console.WriteLine( "SSTcpListener: Dados recebidos OK." );
		//		}
		//		else {
		//			Console.WriteLine( "SSTcpClient: Dados recebidos incompletos." );
		//		}

		//		string received = string.Join( "", contentParts );
		//		Console.WriteLine( "SSTcpListener: Dados recebidos: " + received.Replace( "\n", "\\n" ).Replace( "\r", "" ) );
		//		string response = callback( received );
		//		Console.WriteLine( "SSTcpListener: Dados de resposta: " + response.Replace( "\n", "\\n" ).Replace( "\r", "" ) );

		//		var parts = GetPacketsFromContent( response );
		//		foreach( var part in parts ) {
		//			nwStream.Write( part, 0, part.Length );
		//		}
		//	}
		//	client.Close();
		//}
	}
}
