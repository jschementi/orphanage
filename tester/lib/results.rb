class Results
  attr_reader :tests
  def initialize
    @tests = {}
  end
  def add_describe(desc)
    @tests[desc] = {}
    @cur_desc = desc
  end
  def finish_describe
    @cur_desc = nil
  end
  def add_it(desc)
    if @cur_desc
      @tests[@cur_desc][desc] = {}
      @cur_it = desc
      @tests[@cur_desc][desc][:messages] = []
    end
  end
  def finish_it
    @cur_it = nil
  end
  def add_exception(e)
    if @cur_desc && @cur_it
      @tests[@cur_desc][@cur_it][:exception] = e
    end
  end
  def add_message(msg)
    if @cur_desc && @cur_it
      @tests[@cur_desc][@cur_it][:messages] << msg
    end
  end
end

$results = Results.new

alias :tester_describe :describe
def describe(desc, &b)
  $results.add_describe(desc)
  tester_describe(desc, &b)
  $results.finish_describe
end

alias :tester_it :it
def it(desc, &b)
  $results.add_it(desc)
  tester_it(desc, &b)
rescue => e
  $results.add_exception(e)
ensure
  $expectations.each do |exp|
    unless exp.success
      $results.add_message(exp.message)
      break
    end
  end if $expectations
  $results.finish_it
end

PositiveExpectation.class_eval do
  alias_method :tester_eq, :"=="
  def ==(other)
    msg = tester_eq(other)
    $results.add_message(msg) if msg.kind_of? String
  end
end

at_exit do
  messages = $results.tests.collect do |k,v|
    v.collect do |l,w| 
      w[:messages] + [w[:exception]].flatten.compact
    end
  end.flatten.compact

  passed = $examples - messages.length
  passed = 0 if passed < 0

  puts "\n#{passed} passed, #{messages.length} failed\n\n"

  if messages.length > 0
    puts "Failures: \n\n"
    $results.tests.each do |desc, val|   
      puts "#{desc}"
      val.each do |it, v|
        messages, exception = v[:messages], v[:exception]
        unless messages.empty? && exception.nil?
          puts "  it #{it}"
          messages.each{ |m| puts "  - #{m}" }
          puts "  - #{exception.to_s}\n#{exception.backtrace.join("\n")}" if exception
        end
      end
    end
  end
end
