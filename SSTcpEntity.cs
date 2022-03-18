using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Limcap.SSTcp {
	public abstract class SSTcpEntity {
		public static int packetLength = 10;
		public const int headerLength = 4;

		protected IPAddress ip;
		protected int port;







		protected SSTcpEntity( string ip, int port ) {
			this.ip = ip is null ? IPAddress.Any : IPAddress.Parse( ip == "localhost" ? "127.0.0.1" : ip );
			this.port = port;
		}





		protected void Log( string message, string filename = "SSTcp.log" ) {
			try {
				var nl = Environment.NewLine;
				var text = nl + DateTime.Now.ToString( "dd/MM/yyyy HH:mm:ss" ) + nl + message + nl + nl + "--------------------" + nl;
				System.IO.File.AppendAllText( filename, text );
			}
			catch (Exception ex) { }
		}









		//public static List<byte[]> GetPacketsFromContent( string content )
		//{
		//	content = content.Trim();
		//	if( content.Length == 0 ) return null;

		//	// Obtem os bytes do content
		//	byte[] bodyBytes = Encoding.UTF8.GetBytes( content );

		//	// Calcula o numero de partes.
		//	var qntPackets = bodyBytes.Length <= SSTcpEntity.packetLength - SSTcpEntity.headerLength
		//		? 1
		//		: ( bodyBytes.Length - 1 ) / ( SSTcpEntity.packetLength - SSTcpEntity.headerLength ) + 1;
		//	var qntPackets_Bytes = BitConverter.GetBytes( qntPackets );
		//	if( BitConverter.IsLittleEndian ) Array.Reverse( qntPackets_Bytes );

		//	// Cria a lista que irá conter as partes
		//	var parts = new List<byte[]>( qntPackets );

		//	for( var packetIndex = 1; packetIndex <= qntPackets; packetIndex++ ) {

		//		// Cria o byte[] da parte, vazio, com o comprimento adequado.
		//		int curPacketLength = packetLength;
		//		if( packetIndex == qntPackets ) {
		//			int mod = ( bodyBytes.Length + qntPackets * SSTcpEntity.headerLength ) % SSTcpEntity.packetLength;
		//			if( mod > 0 ) curPacketLength = mod;
		//		}
		//		//int curPacketLength = packetIndex == qntPackets
		//		//	? ( bodyBytes.Length + qntPackets * SSTcpEntity.headerLength ) % SSTcpEntity.packetLength
		//		//	: SSTcpEntity.packetLength;
		//		var packet = new byte[curPacketLength];

		//		// Tansforma o índice da parte atual em byte[].
		//		var packetIndex_Bytes = BitConverter.GetBytes( packetIndex );
		//		if( BitConverter.IsLittleEndian ) Array.Reverse( packetIndex_Bytes );

		//		// Preenche o header da parte.
		//		packet[0] = packetIndex_Bytes[2];
		//		packet[1] = packetIndex_Bytes[3];
		//		packet[2] = qntPackets_Bytes[2];
		//		packet[3] = qntPackets_Bytes[3];

		//		// Preenche o restante da parte com o conteúdo.
		//		for( var j = 4; j < curPacketLength; j++ ) {
		//			int bodyBytes_Index = ( packetIndex - 1 ) * ( SSTcpEntity.packetLength - 4 ) + j - 4;
		//			if( bodyBytes.Length > bodyBytes_Index )
		//				packet[j] = bodyBytes[bodyBytes_Index];
		//			else break;
		//		}

		//		// Adiciona a parte pronta na lista de partes.
		//		parts.Add( packet );
		//	}
		//	var b = parts.Select( p => Encoding.UTF8.GetString( p ) ).ToList();
		//	return parts;
		//}








		//public static List<byte[]> GetPacketsFromContent_Old( string content )
		//{
		//	content = content.Trim();
		//	if( content.Length == 0 ) return null;

		//	// Obtem os bytes do content
		//	byte[] bodyBytes = Encoding.UTF8.GetBytes( content );

		//	// Calcula o numero de partes.
		//	var qntPackets = bodyBytes.Length <= SSTcpEntity.packetLength - SSTcpEntity.headerLength
		//		? 1
		//		: ( bodyBytes.Length - 1 ) / ( SSTcpEntity.packetLength - SSTcpEntity.headerLength ) + 1;
		//	var qntPackets_Bytes = BitConverter.GetBytes( qntPackets );
		//	if( BitConverter.IsLittleEndian ) Array.Reverse( qntPackets_Bytes );

		//	// Cria a lista que irá conter as partes
		//	var parts = new List<byte[]>( qntPackets );

		//	for( var packetIndex = 1; packetIndex <= qntPackets; packetIndex++ ) {

		//		// Cria o byte[] da parte, vazio, com o comprimento adequado.
		//		int curPacketLength = packetLength;
		//		if( packetIndex == qntPackets ) {
		//			int mod = ( bodyBytes.Length + qntPackets * SSTcpEntity.headerLength ) % SSTcpEntity.packetLength;
		//			if( mod > 0 ) curPacketLength = mod;
		//		}
		//		//int curPacketLength = packetIndex == qntPackets
		//		//	? ( bodyBytes.Length + qntPackets * SSTcpEntity.headerLength ) % SSTcpEntity.packetLength
		//		//	: SSTcpEntity.packetLength;
		//		var packet = new byte[curPacketLength];

		//		// Tansforma o índice da parte atual em byte[].
		//		var packetIndex_Bytes = BitConverter.GetBytes( packetIndex );
		//		if( BitConverter.IsLittleEndian ) Array.Reverse( packetIndex_Bytes );

		//		// Preenche o header da parte.
		//		packet[0] = packetIndex_Bytes[2];
		//		packet[1] = packetIndex_Bytes[3];
		//		packet[2] = qntPackets_Bytes[2];
		//		packet[3] = qntPackets_Bytes[3];

		//		// Preenche o restante da parte com o conteúdo.
		//		for( var j = 4; j < curPacketLength; j++ ) {
		//			int bodyBytes_Index = ( packetIndex - 1 ) * ( SSTcpEntity.packetLength - 4 ) + j - 4;
		//			if( bodyBytes.Length > bodyBytes_Index )
		//				packet[j] = bodyBytes[bodyBytes_Index];
		//			else break;
		//		}

		//		// Adiciona a parte pronta na lista de partes.
		//		parts.Add( packet );
		//	}
		//	var b = parts.Select( p => Encoding.UTF8.GetString( p ) ).ToList();
		//	return parts;
		//}
	}
}
