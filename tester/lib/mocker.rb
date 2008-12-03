require 'helper'

$expectations = []

def mock(name)
  name
end

alias :origional_it :it
def it(description, &block)
  $expectations = []
  origional_it(description, &block)
  $expectations.each do |e|
    if e.success then 
      print "." 
    else
      print "E"
      break
    end
  end
end

module MethodExpectations
  def should_receive(method)
    $expectations << (expects = MethodCallExpectation.new(self, method))
    expects.behavior = lambda { expects.success = true }
    create_method(expects)
    return expects
  end

  def with(expects, *args)
    expects.behavior = lambda do |*other_args| 
      expects.success = args == other_args
    end
    create_method(expects)
    return expects
  end

  def and_return(expects, value)
    prev_behavior = expects.behavior
    expects.behavior = lambda do |*args|
      prev_behavior.call(*args)
      return value
    end
    create_method(expects)
  end

  def create_method(expects)
    meta_def expects.method, &(expects.behavior)
  end
end

class MethodCallExpectation 
  attr_accessor :method, :args, :return, :message, :success, :behavior
  
  def initialize(obj, method)
    $examples += 1
    @object = obj
    @method = method
    @success = false
    @message = "#{obj.to_s.split('(').first} should have received #{method}"
  end

  def with(*args)
    @args = args
    @message << " with args (#{args.inspect})"
    @object.with(self, *args)
  end

  def and_return(value)
    @return = value
    @message << " and returns \"#{@return.inspect}\""
    @object.and_return(self, value)
  end
end

class Class
  include MethodExpectations
end

