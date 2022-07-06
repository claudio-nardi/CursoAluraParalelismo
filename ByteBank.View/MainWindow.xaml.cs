using ByteBank.Core.Model;
using ByteBank.Core.Repository;
using ByteBank.Core.Service;
using ByteBank.View.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ByteBank.View
{
    public partial class MainWindow : Window
    {
        private readonly ContaClienteRepository r_Repositorio;
        private readonly ContaClienteService r_Servico;
        private CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();

            r_Repositorio = new ContaClienteRepository();
            r_Servico = new ContaClienteService();
        }

        private async void BtnProcessar_Click(object sender, RoutedEventArgs e)
        {
            var contas = r_Repositorio.GetContaClientes();

            var resultado = new List<string>();

            AtualizarView(new List<string>(), TimeSpan.Zero);

            var inicio = DateTime.Now;

            var contas1 = contas.Take(contas.Count() / 8);
            var contas2 = contas.Skip(contas.Count() / 8).Take(contas.Count() / 8);
            var contas3 = contas.Skip(contas.Count() / 8*2).Take(contas.Count() / 8);
            var contas4 = contas.Skip(contas.Count() / 8*3).Take(contas.Count() / 8);
            var contas5 = contas.Skip(contas.Count() / 8*4).Take(contas.Count() / 8);
            var contas6 = contas.Skip(contas.Count() / 8*5).Take(contas.Count() / 8);
            var contas7 = contas.Skip(contas.Count() / 8*6).Take(contas.Count() / 8);
            var contas8 = contas.Skip(contas.Count() / 8*7);

            Thread thread1 = new Thread(() =>
            {
                foreach (var conta in contas1)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }

            });
            Thread thread2 = new Thread(() =>
            {
                foreach (var conta in contas2)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });
            Thread thread3 = new Thread(() =>
            {
                foreach (var conta in contas3)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });
            Thread thread4 = new Thread(() =>
            {
                foreach (var conta in contas4)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });
            Thread thread5 = new Thread(() =>
            {
                foreach (var conta in contas5)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });
            Thread thread6 = new Thread(() =>
            {
                foreach (var conta in contas6)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });
            Thread thread7 = new Thread(() =>
            {
                foreach (var conta in contas7)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });
            Thread thread8 = new Thread(() =>
            {
                foreach (var conta in contas8)
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                }
            });

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            thread5.Start();
            thread6.Start();
            thread7.Start();
            thread8.Start();

			while (thread8.IsAlive)
			{
                Thread.Sleep(250);
			}

            var fim = DateTime.Now;

            AtualizarView(resultado, fim - inicio);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            BtnCancelar.IsEnabled = false;
            _cts.Cancel();
        }

        private async Task<string[]> ConsolidarContas(IEnumerable<ContaCliente> contas, IProgress<string> reportadorDeProgresso, CancellationToken ct)
        {
            var tasks = contas.Select(conta =>
                Task.Factory.StartNew(() =>
                {
                    ct.ThrowIfCancellationRequested();

                    var resultadoConsolidacao = r_Servico.ConsolidarMovimentacao(conta, ct);

                    reportadorDeProgresso.Report(resultadoConsolidacao);

                    ct.ThrowIfCancellationRequested();
                    return resultadoConsolidacao;
                }, ct)
            );

            return await Task.WhenAll(tasks);
        }

        private void LimparView()
        {
            LstResultados.ItemsSource = null;
            TxtTempo.Text = null;
            PgsProgresso.Value = 0;
        }

        private void AtualizarView(IEnumerable<String> result, TimeSpan elapsedTime)
        {
            var tempoDecorrido = $"{ elapsedTime.Seconds }.{ elapsedTime.Milliseconds} segundos!";
            var mensagem = $"Processamento de {result.Count()} clientes em {tempoDecorrido}";

            LstResultados.ItemsSource = result;
            TxtTempo.Text = mensagem;
        }
    }
}
