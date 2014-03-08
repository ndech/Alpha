using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Alpha.Graphics;
using PlaneSimulator;
using SharpDX.DirectInput;
using SharpDX.Windows;

namespace Alpha
{
    public class Game
    {
        private readonly Timer _timer;
        private readonly Renderer _renderer;
        private readonly Calendar _calendar;
        public Input Input { get; private set; }
        private readonly List<GameComponent> _gameComponents;
        private bool _newRegisteredElement = false;

        private readonly List<Character> _characters;
        private readonly List<Province> _provinces;

        public Game()
        {
            _gameComponents = new List<GameComponent>();
            _characters = new List<Character>();
            _provinces = new List<Province>();
            _calendar = new Calendar(this);
            _calendar.DayChanged += OnDayChanged;
            for(int i = 0; i< 10; i++)
                _characters.Add(new Character());
            for(int i = 0; i< 10; i++)
                _provinces.Add(new Province());
            _timer = new Timer();
            _renderer = new Renderer();
            Input = new Input(this, _renderer.Form.Handle);
            Register(Input);
            Camera camera = new Camera(this); 
            Register(camera);
            _renderer.Camera = camera;
            Register(new MonitoringHeader(this, _renderer));
        }

        public void Register(GameComponent item)
        {
            _newRegisteredElement = true;
            _gameComponents.Add(item);
            if (item is RenderableGameComponent)
                _renderer.Register(item as RenderableGameComponent);
        }

        public void Run()
        {
            RenderLoop.Run(_renderer.Form, () =>
            {
                double delta = _timer.Tick();
                if (_newRegisteredElement)
                {
                    _gameComponents.Sort();
                    _newRegisteredElement = false;
                }

                foreach (GameComponent item in _gameComponents)
                    if(item.Enabled)
                        item.Update(delta);
                
                _calendar.Update(delta);

                if (Input.IsKeyPressed(Key.Escape))
                    Exit();

                if (Input.IsKeyPressed(Key.S))
                    Save();

                if (Input.IsKeyPressed(Key.L))
                    Load();

                if (Input.IsKeyPressed(Key.P))
                    _calendar.Paused = !_calendar.Paused;

                _renderer.Render();

                Console.Clear();
                Console.WriteLine(_calendar.ToString());
                foreach (Character character in _characters)
                    Console.WriteLine(character.ToString());
                foreach (Province province in _provinces)
                    Console.WriteLine(province.ToString());
            });
        }

        private void OnDayChanged()
        {
            foreach (Province province in _provinces)
                province.Update(1.0);
        }

        private void Exit()
        {
            _renderer.Form.Close();
        }

        private void Save()
        {
            Console.WriteLine("Game saved.");
            Directory.CreateDirectory("Saves");
            using (XmlWriter writer = XmlWriter.Create(@"Saves\Alpha_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml"))
	        {
	            writer.WriteStartDocument();
                writer.WriteStartElement("Save");
	            writer.WriteStartElement("Characters");
	            foreach (Character character in _characters)
	                character.Save(writer);
                writer.WriteEndElement();
                writer.WriteStartElement("Provinces");
                foreach (Province province in _provinces)
                    province.Save(writer);
                writer.WriteEndElement();
                writer.WriteEndElement();
	            writer.WriteEndDocument();
	        }
            System.Threading.Thread.Sleep(3000);
        }

        private void Load()
        {
            _characters.Clear();
            _provinces.Clear();
            var myFile = (new DirectoryInfo("Saves")).GetFiles();
            using (XmlReader reader = XmlReader.Create(myFile[0].FullName))
            {
                reader.ReadStartElement("Save");
                reader.ReadStartElement("Characters");
                while (true)
                {
                    if (reader.Name == "") // remove whitespaces and carriage returns
                        reader.Read();
                    else if (reader.Name == "Character")
                        _characters.Add(Character.FromXml((XElement)XNode.ReadFrom(reader)));
                    else
                        break;
                }
                reader.ReadEndElement();
                // Checks for duplicate character id
                if (_characters.GroupBy(x => x.Id).Count(g => g.Count() > 1) > 0)
                    throw new InvalidOperationException("Duplicates character id in save");

                reader.ReadStartElement("Provinces");
                while (true)
                {
                    if (reader.Name == "") // remove whitespaces and carriage returns
                        reader.Read();
                    else if (reader.Name == "Province")
                        _provinces.Add(Province.FromXml((XElement)XNode.ReadFrom(reader), _characters));
                    else
                        break;
                }
                reader.ReadEndElement();
                reader.ReadEndElement();
            }
        }


        public void Dispose()
        {
            _renderer.Dispose();
            foreach (GameComponent item in _gameComponents)
                item.Dispose();
        }
    }
}
