namespace WinFormsApp9
{
    public partial class Form1 : Form
    {
        List<string> icons = new List<string>()
        {
            "!",",","b","Y","v","w","~","N","%","e","a","A","$","[","^","(","<","Z","=","2",
            "!",",","b","Y","v","w","~","N","%","e","a","A","$","[","^","(","<","Z","=","2"
        };
        List<Button> buttons = new List<Button>(); // T�m butonlar i�in bir liste
        Random rnd = new Random();
        int randomindex;
        Button firstClicked = null;
        Button secondClicked = null;
        int player1Score = 0;
        int player2Score = 0;
        int currentPlayer = 1;
        int totalPairs = 20; // 11 �ift oldu�unda oyun bitecek
        int foundPairs = 0; // Bulunan toplam �ift say�s�

        private System.Windows.Forms.Timer hideIconsTimer;
        private System.Windows.Forms.Timer hideTimer;
        private System.Windows.Forms.Timer selectionTimer; // Se�im i�in bir timer
        private bool waitingForConfirmation = false; // Onay bekleme durumu

        public Form1()
        {
            InitializeComponent();
            InitializeButtons(); // Butonlar� ba�lat
            UpdateScoresAndPlayer(); // Skorlar� ve oyuncu s�ras�n� g�ncelle
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Ba�lang��ta butonlara rastgele ikonlar yerle�tirilir
            AssignIconsToButtons();
            ShowAllIcons(); // 5 saniye boyunca ikonlar� g�ster
            hideIconsTimer = new System.Windows.Forms.Timer();
            hideIconsTimer.Interval = 5000; // 5 saniye
            hideIconsTimer.Tick += HideIconsTimer_Tick;
            hideIconsTimer.Start();
        }

        private void InitializeButtons()
        {
            // Formdaki t�m butonlar� bir listeye ekleyelim
            foreach (Button btn in Controls.OfType<Button>())
            {
                buttons.Add(btn);
                btn.Click += Btn_Click;
            }
        }

        private void AssignIconsToButtons()
        {
            // Rastgele ikonlar� butonlara atar
            foreach (Button btn in buttons)
            {
                randomindex = rnd.Next(icons.Count);
                btn.Text = icons[randomindex];
                icons.RemoveAt(randomindex);
            }
        }

        private void ShowAllIcons()
        {
            // T�m butonlar�n ikonlar�n� g�r�n�r yapar
            foreach (Button btn in buttons)
            {
                btn.ForeColor = Color.Black; // �konlar g�r�n�r olsun
            }
        }

        private void HideAllIcons()
        {
            // T�m butonlar�n ikonlar�n� gizler
            foreach (Button btn in buttons)
            {
                btn.ForeColor = btn.BackColor; // �konlar arka planla ayn� renk olsun, gizlensin
            }
        }

        private void HideIconsTimer_Tick(object sender, EventArgs e)
        {
            HideAllIcons(); // �konlar� gizle
            hideIconsTimer.Stop(); // Timer'� durdur
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            // Zaten se�ilmi� iki buton varsa, di�er t�klamalar� engelle
            if (firstClicked != null && secondClicked != null)
                return;

            // E�er buton zaten a��lm��sa, ba�ka bir t�klama yapmay� engelle
            if (clickedButton.ForeColor == Color.Black)
                return;

            // �lk kart se�imi
            if (firstClicked == null)
            {
                firstClicked = clickedButton;
                firstClicked.ForeColor = Color.Black; // �lk butonun ikonu g�r�n�r
                StartSelectionTimer(); // �kinci kart se�imi i�in timer ba�lat�l�r
                return;
            }

            // �kinci kart se�imi
            secondClicked = clickedButton;
            secondClicked.ForeColor = Color.Black; // �kinci butonun ikonu g�r�n�r

            // Timer'� durdur ��nk� ikinci kart se�ildi
            selectionTimer.Stop();

            // E�le�me kontrol� yap
            if (firstClicked.Text == secondClicked.Text)
            {
                // E�er e�le�irse, kartlar a��k kal�r ve puan eklenir
                if (currentPlayer == 1)
                {
                    player1Score++;
                }
                else
                {
                    player2Score++;
                }

                foundPairs++; // Bulunan �ift say�s�

                // E�er t�m �iftler bulunmu�sa veya bir oyuncu 11 �ift bulmu�sa oyun biter
                if (foundPairs == totalPairs || player1Score == totalPairs || player2Score == totalPairs)
                {
                    EndGame(); // Oyunu bitir
                }
                else
                {
                    // Do�ru e�le�meden sonra s�ray� de�i�tirmeden devam edebilir
                    firstClicked = null;
                    secondClicked = null;
                    UpdateScoresAndPlayer(); // Skorlar� g�ncelle
                }
            }
            else
            {
                // E�er e�le�mezse, 1 saniye sonra ikonlar� kapat
                hideTimer = new System.Windows.Forms.Timer();
                hideTimer.Interval = 1000; // 1 saniye sonra gizle
                hideTimer.Tick += HideTimer_Tick;
                hideTimer.Start();

                // S�ray� de�i�tir
                currentPlayer = currentPlayer == 1 ? 2 : 1;

                // Skor ve oyuncu s�ras� g�ncellemesi
                UpdateScoresAndPlayer();

                // S�radaki oyuncuyu ve son skorlar� g�steren pencere
                MessageBox.Show($"Son Skorlar:\nPlayer 1: {player1Score}\nPlayer 2: {player2Score}\n�imdi S�ra: Player {currentPlayer}",
                    "S�ra De�i�ti", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void HideSingleIcon(Button btn)
        {
            if (btn != null)
            {
                btn.ForeColor = btn.BackColor; // Butonun ikonu gizlenir
            }
        }
        private void StartSelectionTimer()
        {
            // �kinci se�im i�in 5 saniye s�re ver
            selectionTimer = new System.Windows.Forms.Timer();
            selectionTimer.Interval = 5000; // 5 saniye
            selectionTimer.Tick += (s, e) =>
            {
                // S�re dolduysa ve ikinci buton se�ilmediyse
                if (secondClicked == null)
                {
                    // S�re doldu, s�ra di�er oyuncuya ge�er
                    MessageBox.Show($"S�re doldu! �imdi s�ra Player {(currentPlayer == 1 ? 2 : 1)}'de");
                    currentPlayer = currentPlayer == 1 ? 2 : 1; // S�ra di�er oyuncuya ge�er
                    UpdateScoresAndPlayer(); // Skorlar� ve s�ray� g�ncelle
                    HideSingleIcon(firstClicked); // Sadece ilk se�ilen butonu gizle
                    firstClicked = null; // �lk butonu s�f�rla
                }

                selectionTimer.Stop(); // Timer'� durdur
            };
            selectionTimer.Start();
        }



        private void HideTimer_Tick(object sender, EventArgs e)
        {
            if (firstClicked != null && secondClicked != null)
            {
                firstClicked.ForeColor = firstClicked.BackColor; // �lk butonun ikonu gizlenir
                secondClicked.ForeColor = secondClicked.BackColor; // �kinci butonun ikonu gizlenir
                firstClicked = null;
                secondClicked = null;
            }

            hideTimer.Stop(); // Timer'� durdur
        }

        // Skor ve oyuncu s�ras� g�ncellemesi
        private void UpdateScoresAndPlayer()
        {
            labelScore.Text = $"Player 1 Score: {player1Score} | Player 2 Score: {player2Score}";
            labelCurrentPlayer.Text = $"Current Player: Player {currentPlayer}";
        }

        private void EndGame()
        {
            // Oyun sona erdi�inde kazanan� g�sterir
            string winner;
            if (player1Score == 11)
            {
                winner = "Player 1";
            }
            else if (player2Score == 11)
            {
                winner = "Player 2";
            }
            else if (player1Score > player2Score && player1Score+player2Score== totalPairs)
            {
                winner = "Player 1";
            }
            else
            {
                winner = "Player 2";
            }

            MessageBox.Show($"{winner} wins!"); // Kazanan� g�sterir
            Application.Exit(); // Oyun sona erer
        }
    }
} 
  