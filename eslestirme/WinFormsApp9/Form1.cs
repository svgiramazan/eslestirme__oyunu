namespace WinFormsApp9
{
    public partial class Form1 : Form
    {
        List<string> icons = new List<string>()
        {
            "!",",","b","Y","v","w","~","N","%","e","a","A","$","[","^","(","<","Z","=","2",
            "!",",","b","Y","v","w","~","N","%","e","a","A","$","[","^","(","<","Z","=","2"
        };
        List<Button> buttons = new List<Button>(); // Tüm butonlar için bir liste
        Random rnd = new Random();
        int randomindex;
        Button firstClicked = null;
        Button secondClicked = null;
        int player1Score = 0;
        int player2Score = 0;
        int currentPlayer = 1;
        int totalPairs = 20; // 11 çift olduðunda oyun bitecek
        int foundPairs = 0; // Bulunan toplam çift sayýsý

        private System.Windows.Forms.Timer hideIconsTimer;
        private System.Windows.Forms.Timer hideTimer;
        private System.Windows.Forms.Timer selectionTimer; // Seçim için bir timer
        private bool waitingForConfirmation = false; // Onay bekleme durumu

        public Form1()
        {
            InitializeComponent();
            InitializeButtons(); // Butonlarý baþlat
            UpdateScoresAndPlayer(); // Skorlarý ve oyuncu sýrasýný güncelle
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Baþlangýçta butonlara rastgele ikonlar yerleþtirilir
            AssignIconsToButtons();
            ShowAllIcons(); // 5 saniye boyunca ikonlarý göster
            hideIconsTimer = new System.Windows.Forms.Timer();
            hideIconsTimer.Interval = 5000; // 5 saniye
            hideIconsTimer.Tick += HideIconsTimer_Tick;
            hideIconsTimer.Start();
        }

        private void InitializeButtons()
        {
            // Formdaki tüm butonlarý bir listeye ekleyelim
            foreach (Button btn in Controls.OfType<Button>())
            {
                buttons.Add(btn);
                btn.Click += Btn_Click;
            }
        }

        private void AssignIconsToButtons()
        {
            // Rastgele ikonlarý butonlara atar
            foreach (Button btn in buttons)
            {
                randomindex = rnd.Next(icons.Count);
                btn.Text = icons[randomindex];
                icons.RemoveAt(randomindex);
            }
        }

        private void ShowAllIcons()
        {
            // Tüm butonlarýn ikonlarýný görünür yapar
            foreach (Button btn in buttons)
            {
                btn.ForeColor = Color.Black; // Ýkonlar görünür olsun
            }
        }

        private void HideAllIcons()
        {
            // Tüm butonlarýn ikonlarýný gizler
            foreach (Button btn in buttons)
            {
                btn.ForeColor = btn.BackColor; // Ýkonlar arka planla ayný renk olsun, gizlensin
            }
        }

        private void HideIconsTimer_Tick(object sender, EventArgs e)
        {
            HideAllIcons(); // Ýkonlarý gizle
            hideIconsTimer.Stop(); // Timer'ý durdur
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            // Zaten seçilmiþ iki buton varsa, diðer týklamalarý engelle
            if (firstClicked != null && secondClicked != null)
                return;

            // Eðer buton zaten açýlmýþsa, baþka bir týklama yapmayý engelle
            if (clickedButton.ForeColor == Color.Black)
                return;

            // Ýlk kart seçimi
            if (firstClicked == null)
            {
                firstClicked = clickedButton;
                firstClicked.ForeColor = Color.Black; // Ýlk butonun ikonu görünür
                StartSelectionTimer(); // Ýkinci kart seçimi için timer baþlatýlýr
                return;
            }

            // Ýkinci kart seçimi
            secondClicked = clickedButton;
            secondClicked.ForeColor = Color.Black; // Ýkinci butonun ikonu görünür

            // Timer'ý durdur çünkü ikinci kart seçildi
            selectionTimer.Stop();

            // Eþleþme kontrolü yap
            if (firstClicked.Text == secondClicked.Text)
            {
                // Eðer eþleþirse, kartlar açýk kalýr ve puan eklenir
                if (currentPlayer == 1)
                {
                    player1Score++;
                }
                else
                {
                    player2Score++;
                }

                foundPairs++; // Bulunan çift sayýsý

                // Eðer tüm çiftler bulunmuþsa veya bir oyuncu 11 çift bulmuþsa oyun biter
                if (foundPairs == totalPairs || player1Score == totalPairs || player2Score == totalPairs)
                {
                    EndGame(); // Oyunu bitir
                }
                else
                {
                    // Doðru eþleþmeden sonra sýrayý deðiþtirmeden devam edebilir
                    firstClicked = null;
                    secondClicked = null;
                    UpdateScoresAndPlayer(); // Skorlarý güncelle
                }
            }
            else
            {
                // Eðer eþleþmezse, 1 saniye sonra ikonlarý kapat
                hideTimer = new System.Windows.Forms.Timer();
                hideTimer.Interval = 1000; // 1 saniye sonra gizle
                hideTimer.Tick += HideTimer_Tick;
                hideTimer.Start();

                // Sýrayý deðiþtir
                currentPlayer = currentPlayer == 1 ? 2 : 1;

                // Skor ve oyuncu sýrasý güncellemesi
                UpdateScoresAndPlayer();

                // Sýradaki oyuncuyu ve son skorlarý gösteren pencere
                MessageBox.Show($"Son Skorlar:\nPlayer 1: {player1Score}\nPlayer 2: {player2Score}\nÞimdi Sýra: Player {currentPlayer}",
                    "Sýra Deðiþti", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // Ýkinci seçim için 5 saniye süre ver
            selectionTimer = new System.Windows.Forms.Timer();
            selectionTimer.Interval = 5000; // 5 saniye
            selectionTimer.Tick += (s, e) =>
            {
                // Süre dolduysa ve ikinci buton seçilmediyse
                if (secondClicked == null)
                {
                    // Süre doldu, sýra diðer oyuncuya geçer
                    MessageBox.Show($"Süre doldu! Þimdi sýra Player {(currentPlayer == 1 ? 2 : 1)}'de");
                    currentPlayer = currentPlayer == 1 ? 2 : 1; // Sýra diðer oyuncuya geçer
                    UpdateScoresAndPlayer(); // Skorlarý ve sýrayý güncelle
                    HideSingleIcon(firstClicked); // Sadece ilk seçilen butonu gizle
                    firstClicked = null; // Ýlk butonu sýfýrla
                }

                selectionTimer.Stop(); // Timer'ý durdur
            };
            selectionTimer.Start();
        }



        private void HideTimer_Tick(object sender, EventArgs e)
        {
            if (firstClicked != null && secondClicked != null)
            {
                firstClicked.ForeColor = firstClicked.BackColor; // Ýlk butonun ikonu gizlenir
                secondClicked.ForeColor = secondClicked.BackColor; // Ýkinci butonun ikonu gizlenir
                firstClicked = null;
                secondClicked = null;
            }

            hideTimer.Stop(); // Timer'ý durdur
        }

        // Skor ve oyuncu sýrasý güncellemesi
        private void UpdateScoresAndPlayer()
        {
            labelScore.Text = $"Player 1 Score: {player1Score} | Player 2 Score: {player2Score}";
            labelCurrentPlayer.Text = $"Current Player: Player {currentPlayer}";
        }

        private void EndGame()
        {
            // Oyun sona erdiðinde kazananý gösterir
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

            MessageBox.Show($"{winner} wins!"); // Kazananý gösterir
            Application.Exit(); // Oyun sona erer
        }
    }
} 
  