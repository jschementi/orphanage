require File.dirname(__FILE__) + '/test_helper'
require 'mscorlib'

include System::Collections

describe '.NET Stack' do
  
  before :each do
    @s = Stack.new
  end

  it 'creates an instance with zero elements' do
    Stack.new.count.should == 0
  end
  
  it 'contains one element after we push' do
    @s.push "bob"
    @s.count.should == 1
  end

  it 'lets us peek the element pushed' do
    @s.push "bob"
    @s.peek.should == 'bob'
    @s.count.should == 1
  end
  
  it 'lets us pop the element we pushed' do
    @s.push "bob"
    @s.pop.should == "bob"
    @s.count.should == 0
  end

end
