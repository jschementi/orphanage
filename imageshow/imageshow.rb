require 'mscorlib'
require 'System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
require 'PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
require 'PresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
require 'WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'

include System
include System::Windows
include System::Windows::Controls
include System::Windows::Media
include System::Windows::Media::Imaging

class ImageShow
  def initialize(urls)
    if urls.empty? || urls.size > 4 || urls.size == 3 
      puts "Must have 1, 2 or 4 urls"
    else
      @urls = urls
      render_images
    end
  end

  def render_images
    puts "Rendering images: #{@urls.inspect}" 
 
    w = Window.new
    w.size_to_content = SizeToContent.width_and_height
    w.background = black
    w.window_style = WindowStyle.tool_window
    w.resize_mode = ResizeMode.NoResize
    w.title = "IronRuby Image Shower"
    
    grid = Grid.new
    grid.background = gray

    width = 10
    size = [ 
      if @urls.size == 4 then width / 2
      elsif @urls.size == 2 then [width / 2, width]
      else width 
      end 
    ].flatten.compact

    add_stack_panel(grid, :vertical, Thickness.new(1)) do |sp|
      add_stack_panel(sp, :horizontal) do |sph|
        add_stack_panel(sph, :vertical, Thickness.new(width, width, size[0], size[-1])) do |spv|
          add_image(@urls[0], spv)
        end
        if @urls.size > 1
          add_stack_panel(sph, :vertical, Thickness.new(size[0], width, width, size[-1])) do |spv|
            add_image(@urls[1], spv)
          end
        end
      end
      if @urls.size == 4
        add_stack_panel(sp, :horizontal) do |sph|
          add_stack_panel(sph, :vertical, Thickness.new(width, size[0], size[-1], width)) do |spv|
            add_image(@urls[2], spv)
          end
          add_stack_panel(sph, :vertical, Thickness.new(size[0], size[-1], width, width)) do |spv|
            add_image(@urls[3], spv)
          end
        end
      end
    end

    w.content = grid
    a = Application.new
    a.run w
  end

  def black
    SolidColorBrush.new(Colors.black)
  end
  
  def gray
    SolidColorBrush.new(Colors.gray)
  end

  def add_image(url, parent)
    image = Image.new
    bimage = BitmapImage.new
    bimage.begin_init
    bimage.uri_source = Uri.new(url)
    bimage.end_init
    image.width = 200
    image.source = bimage
    parent.children.add(image)
  end

  def add_stack_panel(parent, orientation = :vertical, thickness = Thickness.new(0), background = black)
    sph = StackPanel.new
    sph.orientation = Orientation.send(orientation)
    sph.margin = thickness
    sph.background = background
    yield sph
    parent.children.add sph
  end
end

if __FILE__ == $0
  ImageShow.new(ARGV)
end
