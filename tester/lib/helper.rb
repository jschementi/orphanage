class Object
  def meta_def name, &blk
    (class << self; self; end).instance_eval { define_method name, &blk }
  end
end
