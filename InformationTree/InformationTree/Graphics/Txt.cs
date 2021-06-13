using System;
using D = System.Drawing;

namespace FormsGame.Graphics
{
    public class Txt
    {
        #region Properties

        #region Private

        private string fontFamilyName;
        private int blue;
        private int green;
        private int red;
        private double size;
        
        #endregion Private

        #region Public

        public string Text { get; set; }
        public double x { get; private set; }
        public double y { get; private set; }
        public int level { get; private set; } // tree level

        #endregion Public

        #endregion Properties

        #region Constructor

        public Txt(string _text, int _level) : this(_text, 0, 0, 255, 255, 255) { level = _level;  }
        public Txt(string _text, int _x, int _y) : this(_text, _x, _y, 255, 255, 255) { }
        public Txt(string _text, int _x, int _y, int _r, int _g, int _b) : this(_text, _x, _y, "AngelicWar.ttf", 12, _r, _g, _b) { }
        public Txt(string _text, int _x, int _y, string _fontFamilyName, double _size, int _r, int _g, int _b)
        {
            SetText(_text);
            Move(_x, _y);
            SetFont(_fontFamilyName, _size);
            SetColor(_r, _g, _b);
        }

        #endregion Constructor

        #region Methods

        public void Show(D.Graphics graphics)
        {
            var point = new D.PointF((float)x, (float)y);
            var color = (red == 255 && green == 255 && blue == 255) ? D.Color.LightGreen : D.Color.FromArgb(red, green, blue);
            var brush = new D.SolidBrush(color);
            var font = new D.Font(fontFamilyName, (float)size);
            graphics.DrawString(Text.Replace('_', ' '), font, brush, point);
        }

        public void SetFont(string _fontFamilyName, double _size)
        {
            fontFamilyName = _fontFamilyName;
            size = _size;
        }

        public void SetText(string _text)
        {
            Text = _text;
        }

        public void SetColor(int _r, int _g, int _b)
        {
            red = _r;
            blue = _g;
            green = _b;
        }

        public void Move(double _x, double _y)
        {
            x = _x; y = _y;
        }
        
        #endregion Methods

        /*
        void glEnable2D()
        {
	        int vPort[4];
  
	        glGetIntegerv(GL_VIEWPORT, vPort);
  
	        glMatrixMode(GL_PROJECTION);
	        glPushMatrix();
	        glLoadIdentity();
  
	        glOrtho(0, vPort[2], 0, vPort[3], -1, 1);
	        glMatrixMode(GL_MODELVIEW);
	        glPushMatrix();
	        glLoadIdentity();
        }

        void glDisable2D()
        {
	        glMatrixMode(GL_PROJECTION);
	        glPopMatrix();   
	        glMatrixMode(GL_MODELVIEW);
	        glPopMatrix();	
        }
        
        void SDL_GL_RenderText(char *text, 
                              TTF_Font *font,
                              SDL_Color color,
                              SDL_Rect *location)
        {
	        SDL_Surface *initial;
	        SDL_Surface *intermediary;
	        int w,h;
	        GLuint texture;
	
	        // Use SDL_TTF to render our text 
	        initial = TTF_RenderText_Blended(font, text, color);
	
	        //Convert the rendered text to a known format 
	        w = nextpoweroftwo(initial->w);
	        h = nextpoweroftwo(initial->h);
	
	        intermediary = SDL_CreateRGBSurface(0, w, h, 32, 
			        0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000);

	        SDL_BlitSurface(initial, 0, intermediary, 0);
	
	        // Tell GL about our new texture
	        glGenTextures(1, &texture);
	        glBindTexture(GL_TEXTURE_2D, texture);
	        glTexImage2D(GL_TEXTURE_2D, 0, 4, w, h, 0, GL_BGRA, 
			        GL_UNSIGNED_BYTE, intermediary->pixels );
	
	        //GL_NEAREST looks horrible, if scaled...
	        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);	

	        //prepare to render our texture 
	        glEnable(GL_TEXTURE_2D);
	        glBindTexture(GL_TEXTURE_2D, texture);
	        glColor3f(1.0f, 1.0f, 1.0f);
	
	        //Draw a quad at location
	        glBegin(GL_QUADS);
	        //   Recall that the origin is in the lower-left corner
	        //   That is why the TexCoords specify different corners
	        //   than the Vertex coors seem to. 
		        glTexCoord2f(0.0f, 1.0f); 
			        glVertex2f(location->x    , location->y);
		        glTexCoord2f(1.0f, 1.0f); 
			        glVertex2f(location->x + w, location->y);
		        glTexCoord2f(1.0f, 0.0f); 
			        glVertex2f(location->x + w, location->y + h);
		        glTexCoord2f(0.0f, 0.0f); 
			        glVertex2f(location->x    , location->y + h);
	        glEnd();
	
	        //Bad things happen if we delete the texture before it finishes
	        glFinish();
	
	        //return the deltas in the unused w,h part of the rect
	        location->w = initial->w;
	        location->h = initial->h;
	
	        // Clean up
	        SDL_FreeSurface(initial);
	        SDL_FreeSurface(intermediary);
	        glDeleteTextures(1, &texture);
        }

        */
    }
}