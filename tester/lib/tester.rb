$examples = 0
$before = nil

def describe(description)
  yield
end

def it(description)
  $before.call unless $before.nil?
  yield
end

class Object
  def should
    PositiveExpectation.new(self)
  end
end

def before(scope, &block)
  $before = block
end

class PositiveExpectation
  def initialize(obj)
    @obj = obj
  end

  def ==(other)
    $examples += 1
    if @obj != other
      print "E"
      "wanted #{other.inspect}, got #{@obj.inspect}"
    else
      print "."
      true
    end
  end
end

require 'results'
