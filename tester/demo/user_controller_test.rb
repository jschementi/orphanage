require File.dirname(__FILE__) + '/test_helper'

describe "UserController" do

  before :each do
    @controller = UserController.new
    @user = mock("User")
  end

  it "gets all users" do
    User.should_receive(:find).with(:all).and_return [@user]
    @controller.index
    @controller.users.should == [@user]
  end

  it 'gets a user' do
    User.should_receive(:find).with(123).and_return @user
    @controller.show(:id => 123)
    @controller.user.should == @user
  end
  
end


