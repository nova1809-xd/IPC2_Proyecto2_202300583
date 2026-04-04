using System.Text;
using EmisorDrones.CoreBusiness.Models;
using EmisorDrones.CoreBusiness.TDAs;

namespace EmisorDrones.CoreBusiness.Services
{
    public class GraphvizService
    {
        public string GenerarGrafoSistema(SistemaDronesConfig sistema)
        {
            if (sistema == null)
            {
                throw new ArgumentNullException(nameof(sistema));
            }

            StringBuilder dot = new StringBuilder();
            dot.AppendLine("digraph SistemaDrones {");
            dot.AppendLine("  rankdir=TB;");
            dot.AppendLine("  graph [fontname=\"Segoe UI\", bgcolor=\"white\"];");
            dot.AppendLine("  node [fontname=\"Consolas\", shape=plain];");
            dot.AppendLine("  edge [fontname=\"Segoe UI\"];");

            string nombreSistema = EscaparHtml(sistema.NombreSistema);
            dot.AppendLine("  titulo [label=<");
            dot.AppendLine("    <TABLE BORDER=\"0\" CELLBORDER=\"0\" CELLPADDING=\"4\">");
            dot.AppendLine("      <TR><TD><B>Sistema: " + nombreSistema + "</B></TD></TR>");
            dot.AppendLine("    </TABLE>");
            dot.AppendLine("  >];");

            dot.AppendLine("  matriz [label=<");
            dot.AppendLine("    <TABLE BORDER=\"1\" CELLBORDER=\"1\" CELLSPACING=\"0\" CELLPADDING=\"6\">");
            dot.AppendLine("      <TR>");
            dot.AppendLine("        <TD BGCOLOR=\"#EDEDED\"><B>Altura</B></TD>");

            NodoGenerico<ContenidoSistemaDrones> nodoDronHeader = sistema.Contenidos.Cabeza;
            while (nodoDronHeader != null)
            {
                string nombreDron = EscaparHtml(nodoDronHeader.Dato.NombreDron);
                dot.AppendLine("        <TD BGCOLOR=\"#EDEDED\"><B>" + nombreDron + "</B></TD>");
                nodoDronHeader = nodoDronHeader.Siguiente;
            }

            dot.AppendLine("      </TR>");

            int altura = sistema.AlturaMaxima;
            while (altura >= 1)
            {
                dot.AppendLine("      <TR>");
                dot.AppendLine("        <TD BGCOLOR=\"#F7F7F7\"><B>" + altura + "</B></TD>");

                NodoGenerico<ContenidoSistemaDrones> nodoDronFila = sistema.Contenidos.Cabeza;
                while (nodoDronFila != null)
                {
                    string simbolo = ObtenerSimboloEnAltura(nodoDronFila.Dato, altura);
                    string simboloSeguro = EscaparHtml(simbolo);
                    dot.AppendLine("        <TD>" + simboloSeguro + "</TD>");
                    nodoDronFila = nodoDronFila.Siguiente;
                }

                dot.AppendLine("      </TR>");
                altura--;
            }

            dot.AppendLine("    </TABLE>");
            dot.AppendLine("  >];");
            dot.AppendLine("  titulo -> matriz [style=invis];");
            dot.AppendLine("}");

            return dot.ToString();
        }

        public string GenerarGrafoInstrucciones(ListaEnlazada<InstanteTiempo> timeline, string nombreMensaje)
        {
            if (timeline == null)
            {
                throw new ArgumentNullException(nameof(timeline));
            }

            StringBuilder dot = new StringBuilder();
            dot.AppendLine("digraph InstruccionesOptimas {");
            dot.AppendLine("  rankdir=TB;");
            dot.AppendLine("  graph [fontname=\"Segoe UI\", bgcolor=\"white\"];");
            dot.AppendLine("  node [fontname=\"Consolas\", shape=box, style=\"rounded,filled\", fillcolor=\"#F9FBFF\", color=\"#7F8C8D\"];");
            dot.AppendLine("  edge [color=\"#34495E\", arrowsize=0.8];");
            dot.AppendLine("  labelloc=\"t\";");
            dot.AppendLine("  label=\"Mensaje: " + EscaparDot(nombreMensaje) + "\";");

            NodoGenerico<InstanteTiempo> nodoTiempo = timeline.Cabeza;
            int indice = 1;

            while (nodoTiempo != null)
            {
                InstanteTiempo instante = nodoTiempo.Dato;
                string nodeId = "t" + indice;
                StringBuilder label = new StringBuilder();

                label.Append("Tiempo ");
                label.Append(instante.Segundo);
                label.Append("\\l");

                NodoGenerico<AccionDron> nodoAccion = instante.Acciones.Cabeza;
                if (nodoAccion == null)
                {
                    label.Append("sin acciones\\l");
                }
                else
                {
                    while (nodoAccion != null)
                    {
                        label.Append(EscaparDot(nodoAccion.Dato.NombreDron));
                        label.Append(": ");
                        label.Append(EscaparDot(nodoAccion.Dato.Movimiento));
                        label.Append("\\l");
                        nodoAccion = nodoAccion.Siguiente;
                    }
                }

                dot.AppendLine("  " + nodeId + " [label=\"" + label + "\"];");

                if (indice > 1)
                {
                    string anteriorId = "t" + (indice - 1);
                    dot.AppendLine("  " + anteriorId + " -> " + nodeId + ";");
                }

                nodoTiempo = nodoTiempo.Siguiente;
                indice++;
            }

            if (indice == 1)
            {
                dot.AppendLine("  vacio [label=\"sin timeline\", shape=note, fillcolor=\"#FFF8E1\"];");
            }

            dot.AppendLine("}");
            return dot.ToString();
        }

        private string ObtenerSimboloEnAltura(ContenidoSistemaDrones contenido, int altura)
        {
            NodoGenerico<AlturaSimbolo> nodoAltura = contenido.Alturas.Cabeza;

            while (nodoAltura != null)
            {
                if (nodoAltura.Dato.ValorAltura == altura)
                {
                    return nodoAltura.Dato.Simbolo;
                }

                nodoAltura = nodoAltura.Siguiente;
            }

            return "-";
        }

        private string EscaparHtml(string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return string.Empty;
            }

            return texto
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;");
        }

        private string EscaparDot(string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return string.Empty;
            }

            return texto
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }
    }
}
