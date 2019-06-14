using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace win_PokeDex
{
    public partial class Form1 : Form
    {
        //Set the Uri component in app settings by clicking on App.Config file
        //Under configuration add the <appsettings> </appsettings> tags
        //<add key="Url" value="https://pokeapi.co/api/v2/pokemon/"/> This will be the URL we make a request to get our data
        //The Uri is then set to the Url in the appsettings
        private string Uri = ConfigurationManager.AppSettings["Url"];

        //This will be used to hold the user input in the textbox
        private string pokemonName;

        //This string will be used to hold our data response.
        private string response = "";

        private List<string> abilities = new List<string>();


        public Form1()
        {
            InitializeComponent();
        
        }

        //Create a button, name it to whatever you want 
        private void button1_Click(object sender, EventArgs e)
        {
            //We create an lblException label and set its visibilty to false so that it only appears in case of 
            //an exception
           

            //The pokemon name has to be all lowercase so we set it by user .ToLower()
            pokemonName = txtNameSearch.Text.ToLower();

            //Put our web request in a try catch
            try
            {
                //Creating our web request: A combination of our url plus the pokemon name
                HttpWebRequest pokeRequest = (HttpWebRequest)WebRequest.Create(Uri + pokemonName);
                //Specify the content we will get in response (JSON)
                pokeRequest.ContentType = "application/json";
                //Keep the request alive
                pokeRequest.KeepAlive = true;
                //Specify the method (GET)
                pokeRequest.Method = "GET";

                //Our response will be the request response
                HttpWebResponse pokeResponse = (HttpWebResponse)pokeRequest.GetResponse();

                //If our response returns an OK status code...Proceed. Else write the exception
                if (pokeResponse.StatusCode == HttpStatusCode.OK)
                {
                    //We use a streamreader to read our response to our empty string
                    using (StreamReader sr = new StreamReader(pokeResponse.GetResponseStream()))
                    {
                        response = sr.ReadToEnd();
                    }

                    //We set the variable equal to deserialize the PokemonInfo. However we specify the RootObject subclass
                    var pokemonContainer = JsonConvert.DeserializeObject<PokemonInfo.RootObject>(response);

                    //Set the textboxes equal to the attribute of the class
                    lblPokeName.Text = pokemonContainer.name.ToString();
                    lblPokeHeight.Text = pokemonContainer.height.ToString();
                    lblPokeWeight.Text = pokemonContainer.weight.ToString();

                    imgPokemon.ImageLocation = pokemonContainer.sprites.front_default;
                    //Using this webclient we download the client file (FOR FUN) then we set the image equal to the sprite image URL
                 

                    
                    //We want to get all the moves the pokemon has/can learn into our ability combobox
                    //So we iterate through each of the abilities using a foreach loop 
                    //We go through and specify a variable to hold each object within the pokemoncontainer.moves
                    foreach (var item in pokemonContainer.moves)
                    {
                        //To Add to a combobox we use the name then .Items then . then Add(item to add goes here)
                        cbAbility.Items.Add(item.move.name.ToString());
                        abilities.Add(item.move.name.ToString());
                        
                    }

                    
                    //Set the first selected item to the first item in the combobox!
                    cbAbility.SelectedItem = cbAbility.Items[0];

                    //We can call a function now to write everything to a Text file
                    //Make sure to pass the information as parameters to our method
                    WritePokemonToFile(pokemonContainer.name, pokemonContainer.height.ToString(), pokemonContainer.weight.ToString(),abilities,pokemonContainer.sprites.front_default);
                }

                //Else our exception message
                else
                {
                    MessageBox.Show("Could not find the resource","Information", MessageBoxButtons.OKCancel,MessageBoxIcon.Error);
                }




            }

            //else our other exception message
            catch (Exception exception)
            {

                MessageBox.Show(exception.ToString(), "Error", MessageBoxButtons.OKCancel,MessageBoxIcon.Error);
            }
        }

        //Our method to write pokemon info to text file  and download the image to a jpeg file
        //Match the parameters in the places where you call this function
        public void WritePokemonToFile(string pokemonName, string pokemonHeight, string pokemonWeight, List<string> abilities, string pokemonImage)
        {
            //Our file name for the text files will be right on our desktop
            string filename = @"C:\Users\diyarkarim\desktop\" + pokemonName + ".txt";

            //Using a streamwriter we can write all our information to our text file
            //The true here means append any data, and false means delete and rewrite: Set to false
            //We dont want a million of these...
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                //Because we are using the Using statement no need to remember to close the writer or anything
                sw.WriteLine("Pokemon Name: {0}", pokemonName);
                //Lines are just for spacing
                sw.WriteLine("===========================================");
                sw.WriteLine("Pokemon Height (ft): {0} \r\n", pokemonHeight);
                sw.WriteLine("Pokemon Weight (lb): {0} \r\n", pokemonWeight);
                sw.WriteLine("===========================================");

                //foreach each of our abilities write ability and the name of it
                foreach (string item in abilities)
                {
                    sw.WriteLine("Ability: {0}", item);
                }

                sw.WriteLine("===========================================");


            }

            //Using our webclient we create an image file by using our image url then the path
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(pokemonImage, @"C: \Users\diyarkarim\desktop\" + pokemonName + ".jpg");
            }
         

            
        }

        //Clear Fields function
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtNameSearch.Text = string.Empty;
            lblPokeHeight.Text = string.Empty;
            lblPokeName.Text = string.Empty;
            lblPokeWeight.Text = string.Empty;
            imgPokemon.Image = null;
            
            for (int i = 0; i < cbAbility.Items.Count; i++)
            {
                cbAbility.Items[i] = string.Empty;
                cbAbility.Items.Remove(i);
            }

            txtNameSearch.Focus();
            
        }
    }
}
