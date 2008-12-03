$LOAD_PATH << '..'
require 'tester'

class ToTest
  def truth
    42
  end
  def network
    Network.open('http://localhost:1234')
  end
end
class Network; end

describe ToTest do
  before(:each) do 
    @t = ToTest.new
  end
  it 'should return 42' do
    @t.truth.should == 42
  end
  it 'should open a network connection' do
    Network.should_receive(:open).with('http://localhost:1234').and_return true
    @t.network.should == true
  end
end
