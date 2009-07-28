require 'rubygems'
require 'test/spec'

require 'mscorlib'
include System::Collections

describe '.NET Stack instantiation' do
  it 'can create an empty stack' do
    @stack = Stack.new
    @stack.should.be.a.kind_of System::Collections::Stack
    @stack.count.should == 0
  end

  it 'can create a stack from an array' do
    @stack = Stack.new [1,2,3]
    @stack.should.be.a.kind_of System::Collections::Stack
    @stack.count.should == 3
  end
end

describe '.NET Stack operations' do
  before :each do
    @stack = Stack.new [1,2,3]
  end

  it 'can push onto the stack' do
    @stack.push 4
    @stack.peek.should == 4
  end

  it 'can pop off of the stack' do
    @stack.pop.should == 3
    @stack.pop.should == 2
    @stack.pop.should == 1
    lambda{ @stack.pop }.should.raise TypeError
  end

  it 'can peek' do
    @stack.peek.should == 3
    @stack.pop
    @stack.peek.should == 2
  end

  it 'can clear' do
    @stack.clear
    @stack.count.should == 0
  end
end
