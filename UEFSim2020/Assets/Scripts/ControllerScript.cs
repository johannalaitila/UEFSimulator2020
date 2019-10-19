﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace UEFSimulator
{
    public class ControllerScript : MonoBehaviour
    {
        public static bool GameOver = false;
        public int TukiKuukaudet, Nopat, Rahat, VapaaAika, Nalka, Kofeiini, Motivaatio, Psykoosi, MotivationPenalty, MonthlyAllowance, Kolikot;
        public bool RakkausElama, ZynZyn;
        public Text TukiKuukaudetText, NopatText, RahatText, VapaaAikaText, OlutText, NalkaText, KofeiiniText, MotivaatioText, PsykoosiText, RakkausElamaText;
        public GameObject PopupImage, RigidBodyFPSController, GameOverImage;
        public AudioClip ShotgunSound, EatSound, DiceSound, GameSound, StomachSound, VomitSound, VictorySound, OluttaSound, BottleSound, VendSound, Value4Life, UefBiisi, ZynBiisi;
        public float Promillet = 0.0f;
        public Image OluttaImage;
        public GameObject Radio;

        private bool virgin = true;

        private AudioSource audioSource;

        private int popupMonth = 0;
        private float secondTime = 0.1f;
        private float secondTimer = 0.0f;
        private float dayTime = 2.5f;
        private float dayTimer = 0.0f;
        private float monthTime = 6.0f;
        private float monthTimer = 0.0f;

        public static bool popupActive = false;
        private RigidbodyFirstPersonController fpsController;

        private bool ekaVuosiPopUp, tokaVuosiPopup, kolmasVuosiPopup, neljasVuosiPopup;

        //private bool GameStarted = false;
        // Start is called before the first frame update
        void Start()
        {
            popupActive = false;
            PopupImage.SetActive(false);
            audioSource = GetComponent<AudioSource>();
            fpsController = RigidBodyFPSController.GetComponent<RigidbodyFirstPersonController>();

            ShowPopup("Hei fuksipallero! Valitettavasti siivoojat nukkuivat tänään pommiin, joten tietokoneluokassa on hieman sotkuista.\n\nPaina Enter esittääksesi välittävän siistiydestä.");
        }

        // Update is called once per frame
        void Update()
        {
            if (GameOver) {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    GameOver = false;
                    SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
                }
            }
            else
            {
                updateUIText();

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    PopupImage.SetActive(false);
                    popupActive = false;
                }

                secondTimer += Time.deltaTime;
                if (secondTimer > secondTime)
                {
                    DecreaseMoney(3);
                    secondTimer = 0;
                    
                }

                dayTimer += Time.deltaTime;
                if (dayTimer > dayTime)
                {
                    IncreaseHunger();
                    dayTimer = 0;
                    Promillet -= 3.5f;
                    //if (Promillet < 50) RakkausElama = false;
                    if (Promillet < 0) Promillet = 0;
                }

                monthTimer += Time.deltaTime;
                if (monthTimer > monthTime)
                {
                    Rahat += MonthlyAllowance;
                    DecreaseMonths();
                    monthTimer = 0;
                }


                //if (increasePopupTimer) timerBeforeNextPopupAllowed += Time.deltaTime;
                if (Promillet < 10)
                {
                    fpsController.movementSettings.ForwardSpeed = 8.0f;
                    OluttaImage.color = new Color32(255, 255, 255, 0);
                }
                if (Promillet > 10)
                {
                    fpsController.movementSettings.ForwardSpeed = 6.0f;
                    OluttaImage.color = new Color32(255, 255, 255, 60);
                }
                if (Promillet > 20)
                {
                    fpsController.movementSettings.ForwardSpeed = 4.5f;
                    OluttaImage.color = new Color32(255, 255, 255, 120);
                }
                if (Promillet > 30)
                {
                    fpsController.movementSettings.ForwardSpeed = 2.5f;
                    OluttaImage.color = new Color32(255, 255, 255, 180);
                }
                var asd = Promillet * 5;
                
                try {
                    byte byteasd = System.Convert.ToByte(asd);
                    if (byteasd > 210) byteasd = 210;
                    OluttaImage.color = new Color32(255, 255, 255, byteasd);
                }
                catch
                {
                    OluttaImage.color = new Color32(255, 255, 255, 210);
                }
                
            }
        }

        void updateUIText()
        {
            TukiKuukaudetText.text = "Tukikuukausia jäljellä: " + TukiKuukaudet + " kk";
            NopatText.text = "Nopat: " + Nopat + " op";
            RahatText.text = "Rahat: " + Rahat + "€";
            VapaaAikaText.text = "Vapaa-aika: " + VapaaAika + " min";
            OlutText.text = "Promillet: " + System.Math.Round(Promillet / 30, 2);

            NalkaText.text = "Nälkä: " + Nalka;
            KofeiiniText.text = "Kofeiini: " + Kofeiini + " mg";
            MotivaatioText.text = "Motivaatio: " + Motivaatio + "%";
            PsykoosiText.text = "Psykoosi: " + Psykoosi + "%";
            if (RakkausElama) RakkausElamaText.text = "Rakkauselämä: olutta";
            else if(!virgin) RakkausElamaText.text = "Rakkauselämä: ollutta";
            else RakkausElamaText.text = "Rakkauselämä: ei";
        }

        #region PlayerActions
        public void ChangeRadio()
        {
            Radio.GetComponent<AudioSource>().Stop();

            if(Radio.GetComponent<AudioSource>().clip == ZynBiisi)
            {
                Radio.GetComponent<AudioSource>().clip = UefBiisi;
            }
            else if (Radio.GetComponent<AudioSource>().clip == UefBiisi)
            {
                Radio.GetComponent<AudioSource>().clip = Value4Life;
            }
            else {
                Radio.GetComponent<AudioSource>().clip = ZynBiisi;
            }

            Radio.GetComponent<AudioSource>().Play();
        }

        public void Study()
        {
            PlaySound(DiceSound);

            int rand = Random.Range(1, 6);
            if (rand != 4)
                IncreaseDice();
            else
            {
                IncreasePsychosis();

                int randS = Random.Range(1, 7);
                if (randS == 1) ShowPopup("Et päässyt kurssia läpi, koska olit liian darrassa tentissä!\n\nPaina Enter vittuuntuaksesi.");
                else if (randS == 2) ShowPopup("Et päässyt kurssia läpi, koska olet liian tyhmä!\n\nPaina Enter vittuuntuaksesi.");
                else if (randS == 3) ShowPopup("Et päässyt kurssia läpi, koska et ole professorin suosikkilistalla.\n\nPaina Enter masentuaksesi.");
                else if (randS == 4) ShowPopup("Et päässyt kurssia läpi, koska vietit liikaa aikaa Jolenessa.\n\nPaina Enter masentuaksesi.");
                else if (randS == 5) ShowPopup("Et päässyt kurssia läpi, koska x-tehtävät menivät tunteisiin.\n\nPaina Enter masentuaksesi.");
                else if (randS == 6) ShowPopup("Et päässyt kurssia läpi, koska ICT-opintopolun opetus ei vastaa tentissä vaadittua osaamista.\n\nPaina Enter masentuaksesi.");
                else if (randS == 7) ShowPopup("Et päässyt kurssia läpi, koska professorin kissat söivät läksysi.\n\nPaina Enter masentuaksesi.");


                
            }

            DecreaseMotivation();
            DecreaseFreeTime();
            DecreaseCaffeine();

            IncreaseHunger();
            IncreasePsychosis();

            
            RakkausElama = false;
        }

        public void Bottle()
        {
            PlaySound(BottleSound);
            Kolikot++;
            ShowPopup("Palautit pullon kauppaan!\n\nPaina Enter saadaksesi rahaa.");
        }

        public void Eat()
        {
            if (Rahat >= 4)
            {
                PlaySound(EatSound);
                DecreaseMoney(4);
                DecreaseHunger();

                int randNumber = Random.Range(1, 35);
                // Random events from money running out
                if (randNumber == 13)
                {
                    PlaySound(VomitSound);
                    ShowPopup("Sait Kemeristä vatsataudin, joka on tappava. Game over!\n\nPaina Enter uudelleensyntyäksesi fuksipallerona.");
                    LoseGame();
                }

                popupMonth = TukiKuukaudet;
                RakkausElama = false;
            }
        }

        public void Drink()
        {
            if (Rahat >= 45)
            {
                PlaySound(OluttaSound);
                DecreaseMoney(45);
                DecreasePsychosis();
                IncreaseBeer();
                IncreaseHunger();
                if (Promillet > 30)
                {
                    int rand = Random.Range(1, 5);
                    if (rand == 1)
                    {
                        
                        int rand2 = Random.Range(1, 4);
                        if(rand2 == 1) ShowPopup("Kompastuit karaokelavalta astuessasi pois ja lensit hurmaavan henkilön päälle!\n\nPaina Enter saadaksesi klamydian.");
                        else if (rand2 == 2) ShowPopup("Kompastuit karaokelavalta astuessasi pois ja lensit hurmaavan henkilön päälle!\n\nPaina Enter saadaksesi tippurin.");
                        else if (rand2 == 3) ShowPopup("Kompastuit karaokelavalta astuessasi pois ja lensit hurmaavan henkilön päälle!\n\nPaina Enter saadaksesi HIV:n.");
                        else ShowPopup("Kompastuit karaokelavalta astuessasi pois ja lensit hurmaavan henkilön päälle!\n\nPaina Enter saadaksesi kondylooman.");
                        RakkausElama = true;
                        virgin = false;
                    }
                    else if (rand == 2)
                    {
                        ShowPopup("Törmäsit suosikkiprofessoriisi ollessasi Jolenessa.\n\nPaina Enter vastaanottaaksesi ylimääräisiä noppia vapaa-ajan aktiivisuudesta.");
                        IncreaseDice();
                        IncreaseDice();
                    }
                }
                else RakkausElama = false;
            }
        }

        public void Work()
        {
            if (Nopat > 180)
            {
                Rahat += 200;
                IncreaseHunger();
                IncreasePsychosis();
                DecreaseMotivation();
                IncreasePsychosis();
                DecreaseMotivation();
            }
            else
            {
                int rand = Random.Range(1, 7);
                if(rand == 1) ShowPopup("Sinulla ei ole tarpeeksi opintopisteitä, joten kukaan ei halua palkata sinua.\n\nPaina Enter masentuaksesi.");
                else if (rand == 2) ShowPopup("Sinulla ei ole tarpeeksi työkokemusta, joten kukaan ei halua palkata sinua.\n\nPaina Enter masentuaksesi.");
                else if (rand == 3) ShowPopup("Olet liian ruma, joten kukaan ei halua palkata sinua.\n\nPaina Enter masentuaksesi.");
                else if (rand == 4) ShowPopup("Oksensit haastattelijan kengille, joten sinua ei palkattu.\n\nPaina Enter masentuaksesi.");
                else if (rand == 5) ShowPopup("Työhaastattelijan koira söi työhakemuksesi.\n\nPaina Enter masentuaksesi.");
                else if (rand == 6) ShowPopup("Insert jokin tekosyy miksi et pääse töihin.\n\nPaina Enter masentuaksesi.");
                else if (rand == 7) ShowPopup("Et sovi työyhteisön sosiaalisiin normeihin.\n\nPaina Enter masentuaksesi.");
            }
        }

        public void IncreaseBeer()
        {
            Promillet += 7;
            if(Promillet > 50)
            {

            }
        }

        public void Game()
        {
            PlaySound(GameSound);
            IncreaseMotivation();
            IncreaseHunger();
            RakkausElama = false;
        }

        public void Vending()
        {
            if (Kolikot > 0)
            {
                PlaySound(VendSound);
                Kolikot--;
                Kofeiini += 10;
                ShowPopup("Slurp. Ostit energiajuomaa.\n\nPaina Enter päristäksesi.");
            }
            else
            {
                ShowPopup("Sinulla ei ole kolikoita mukanasi, joten et voi ostaa energiajuomaa.\n\nPaina Enter kärsiäksesi vieroitusoireista.");
            }
        }
        #endregion

        private void ShowPopup(string text)
        {
            PopupImage.SetActive(true);
            popupActive = true;
            PopupImage.GetComponentInChildren<Text>().text = text;
        }

        #region Modify stats
        private void DecreaseMonths()
        {
            if (TukiKuukaudet > 0)
            {
                TukiKuukaudet--;

            }
            else
            {
                MonthlyAllowance = 0;
                ShowPopup("Oho! Tukikuukaudet loppuivat. Kela ei enää sponsoroi sinua.");
            }
        }

        private void DecreaseMotivation()
        {
            Motivaatio -= 1 + (1* MotivationPenalty);
            if (Motivaatio <= 0)
            {
                Motivaatio = 0;
                PlaySound(ShotgunSound);
                ShowPopup("Motivaatiosi opiskella loppui. Hävisit pelin!\n\nPaina Enter uudelleensyntyäksesi fuksipallerona.");
                LoseGame();
            }
        }

        private void DecreaseFreeTime()
        {
            VapaaAika--;
            // Modify MotivationPenalty in here
            //if (VapaaAika <= 0) GameOver();
        }

        private void DecreaseCaffeine()
        {
            Kofeiini--;
        }

        private void DecreaseMoney(int value)
        {
            if(Rahat == 0) return;
            Rahat -= value;
            if (Rahat <= 0)
            {
                Rahat = 0;
                //if (popupActive) return;
                int randNumber = Random.Range(1, 20);
                popupActive = true;
                
                // Random events from money running out
                if (randNumber == 7)
                {
                    ShowPopup("Rahat loppuivat!\n\nPaina Enter myydäksesi munuaisen.");
                    Rahat += 5000;
                }
                else
                {
                    ShowPopup("Hups! Rahat loppuivat.\n\nPaina Enter syödäksesi loppukuun nuudeleita ilman tonnikalaa.");
                }
                popupMonth = TukiKuukaudet;
            }
        }

        private void IncreaseHunger()
        {
            Nalka += 3;
            if(Nalka > 60 && Nalka < 70) PlaySound(StomachSound);
            if (Nalka >= 100)
            {
                PlaySound(StomachSound);
                ShowPopup("Kuolit nälkään. Olisit käynyt Kemerissä!\n\nPaina Enter uudelleensyntyäksesi fuksipallerona.");
                LoseGame();
            }
        }

        private void DecreaseHunger()
        {
            Nalka -= 40;
            if (Nalka < 0) Nalka = 0;
        }

        private void DecreasePsychosis()
        {
            Psykoosi -= 10;
            if (Psykoosi < 0)
            {
                Psykoosi = 0;
            }
        }

        private void IncreasePsychosis()
        {
            Psykoosi += 2;
            if (Psykoosi >= 100)
            {
                ShowPopup("Liian kovat psykoosit tulilla! Hävisit pelin.\n\nPaina Enter uudelleensyntyäksesi fuksipallerona.");
                LoseGame();
            }
        }

        private void IncreaseDice()
        {
            Nopat += 4;
            if (Nopat >= 60 && !ekaVuosiPopUp)
            {
                ShowPopup("Pääsit fuksivuoden opinnoista onnistuneesti läpi.\n\nPaina Enter hajotaksesi tuleviin x-tehtäviin.");
                ekaVuosiPopUp = true;
            }
            else if (Nopat >= 120 && !tokaVuosiPopup)
            {
                ShowPopup("Pääsit kakkosvuoden opinnoista onnistuneesti läpi.\n\nPaina Enter hajotaksesi tulevaan kandin kirjoittamiseen.");
                tokaVuosiPopup = true;
            }
            else if (Nopat >= 180 && !kolmasVuosiPopup)
            {
                ShowPopup("Suoritit luonnontieteiden kandidaatin tutkinnon onnistuneesti.\n\nPaina Enter hajotaksesi tulevaan kissojen älykkyyden kurssiin.");
                kolmasVuosiPopup = true;
            }
            else if (Nopat >= 240 && !neljasVuosiPopup)
            {
                ShowPopup("Pääsit neljännen vuoden opinnoista onnistuneesti läpi.\n\nPaina Enter hajotaksesi tulevaan gradun kirjoittamiseen.");
                neljasVuosiPopup = true;
            }
            else if (Nopat >= 300)
            {
                ShowPopup("Onnittelut! Toisin kuin suurin osa opiskelijoista, sinä valmistuit ajoissa. Amanuenssi on tyytyväinen.\n\nPaina Enter voittaaksesi pelin.");
                audioSource.Stop();
                PlaySound(VictorySound);
                LoseGame();
            }
           
        }

        private void IncreaseMotivation()
        {
            Motivaatio += 4;
            if (Motivaatio > 100) Motivaatio = 100;
        }

        #endregion
        private void LoseGame()
        {
            GameOverImage.SetActive(true);
            PopupImage.SetActive(true);
            Radio.GetComponent<AudioSource>().Stop();
            GameOver = true;
            RigidBodyFPSController.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        }

        private void PlaySound(AudioClip clip)
        {
            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.PlayOneShot(clip);
        }
    }
}