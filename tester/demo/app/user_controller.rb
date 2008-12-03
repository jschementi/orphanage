# models -------------------------------------

# ActiveRecord stub
module ActiveRecord
  class Base
  end
end

class User < ActiveRecord::Base
end

# controllers --------------------------------

# ActionController stub
module ActionController
  class Base
  end
end

class UserController < ActionController::Base
  attr_reader :user, :users

  def index
    @users = User.find(:all)
  end

  def create(params)
    User.create(params[:user])
  end

  def edit(params)
    show(params)
  end

  def update(params)
    @user = User.find(params[:id])
    if @user
      @user.update_attributes(params[:user])
    end
  end

  def show(params)
    @user = User.find(params[:id])
  end

  def destroy(params)
    User.destroy(params[:id])
  end

end
